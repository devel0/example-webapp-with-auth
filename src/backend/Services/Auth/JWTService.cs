namespace ExampleWebApp.Backend.WebApi.Services.Auth;

public class JWTService : IJWTService
{
    readonly ILogger<JWTService> logger;
    readonly IConfiguration configuration;
    readonly UserManager<ApplicationUser> userManager;
    readonly AppDbContext dbContext;

    static SemaphoreSlim semRefreshToken = new SemaphoreSlim(1, 1);

    public JWTService(
        ILogger<JWTService> logger,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.userManager = userManager;
        this.dbContext = dbContext;
    }

    public TimeSpan AccessTokenLifetime => configuration.GetAppConfig().Auth.Jwt.AccessTokenDuration;

    public TimeSpan RefreshTokenLifetime => configuration.GetAppConfig().Auth.Jwt.RefreshTokenDuration;

    public SymmetricSecurityKey JwtEncryptionKey =>
        new SymmetricSecurityKey(Convert.FromBase64String(configuration.GetAppConfig().Auth.Jwt.Key));

    public string Issuer => configuration.GetAppConfig().Auth.Jwt.Issuer;

    public string Audience => configuration.GetAppConfig().Auth.Jwt.Audience;

    public JwtSecurityToken? DecodeAccessToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadToken(accessToken) as JwtSecurityToken;
    }
   
    public void AddAccessTokenToHttpResponse(HttpResponse response, AccessTokenNfo accessTokenNfo)
    {
        response.Cookies.Append(
            WEB_CookieName_XAccessToken,
            accessTokenNfo.AccessToken,
            new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = accessTokenNfo.Expiration
            });
    }

    public void AddRefreshTokenToHttpResponse(HttpResponse response, RefreshTokenNfo refreshTokenNfo)
    {
        response.Cookies.Append(
            WEB_CookieName_XRefreshToken,
            refreshTokenNfo.RefreshToken,
            new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenNfo.Expiration
            });
    }

    public string? GetAccessTokenFromHttpRequest(HttpRequest request) =>
        request.Cookies[WEB_CookieName_XAccessToken];

    public string? GetRefreshTokenFromHttpRequest(HttpRequest request) =>
        request.Cookies[WEB_CookieName_XRefreshToken];

    public void DeleteAccessTokenFromReponse(HttpResponse response)
    {
        response.Cookies.Delete(WEB_CookieName_XAccessToken);
    }

    public void DeleteRefreshTokenFromReponse(HttpResponse response)
    {
        response.Cookies.Delete(WEB_CookieName_XRefreshToken);
    }

    public bool IsAccessTokenValid(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var config = configuration.GetAppConfig();

            var res = handler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config.Auth.Jwt.Issuer,
                ValidAudience = config.Auth.Jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(config.Auth.Jwt.Key)),
                ClockSkew = config.Auth.Jwt.ClockSkew
            }, out SecurityToken validatedToken);

            return true;
        }

        catch
        {
            return false;
        }
    }

    public async Task<string?> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var utcNow = DateTimeOffset.UtcNow;

        var qrefresh = await dbContext.UserRefreshTokens
            .FirstOrDefaultAsync(w => w.RefreshToken == refreshToken && w.Expires > utcNow, cancellationToken);

        if (qrefresh is null)
            return null;

        return qrefresh.UserName;
    }

    public AccessTokenNfo GenerateAccessToken(string username, string email, IList<Claim> claims)
    {
        var key = JwtEncryptionKey;

        var expires = DateTime.UtcNow.Add(AccessTokenLifetime);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(JwtEncryptionKey, JWT_SecurityAlghoritm)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var accessToken = tokenHandler.WriteToken(token);

        // logger.LogDebug($"generated access token : {accessToken}");

        return new AccessTokenNfo(accessToken, expires);
    }

    public async Task<RefreshTokenNfo> GenerateRefreshTokenAsync(string userName, CancellationToken cancellationToken)
    {
        string refreshToken;

        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            refreshToken = Convert.ToBase64String(randomNumber);
        }

        var utcNow = DateTimeOffset.UtcNow;

        var newRt = new UserRefreshToken
        {
            Issued = utcNow,
            Expires = utcNow + RefreshTokenLifetime,
            RefreshToken = refreshToken,
            UserName = userName
        };

        await semRefreshToken.WaitAsync(cancellationToken);
        try
        {
            dbContext.UserRefreshTokens.Add(newRt);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            semRefreshToken.Release();
        }

        return new RefreshTokenNfo(refreshToken, newRt.Expires);
    }

    public async Task MaintenanceRefreshTokenAsync(string userName, CancellationToken cancellationToken)
    {
        var qRefreshTokensToPurge = dbContext.UserRefreshTokens
            .Where(r => r.UserName == userName && r.Expires < DateTimeOffset.UtcNow)
            .ToList();

        // logger.LogTrace($"purging {qRefreshTokensToPurge.Count} refresh tokens");

        if (qRefreshTokensToPurge.Count > 0)
        {
            await semRefreshToken.WaitAsync(cancellationToken);
            try
            {
                dbContext.RemoveRange(qRefreshTokensToPurge);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                semRefreshToken.Release();
            }
        }
    }


    public async Task<AccessTokenNfo?> RenewAccessTokenAsync(
        string refreshToken, CancellationToken cancellationToken)
    {
        var username = await IsRefreshTokenValidAsync(refreshToken, cancellationToken);
        if (username is null) return null;

        var quser = dbContext.Users.FirstOrDefault(w => w.UserName == username);
        if (quser is null) return null;

        var dtNow = DateTime.UtcNow;
        var userIsLockedOut = await userManager.IsLockedOutAsync(quser);
        var userDisabled = quser.Disabled == true;

        if (userIsLockedOut || userDisabled) return null;

        var email = quser.Email;
        if (email is null) throw new Exception("email not set");

        var resToken = GenerateAccessToken(username, email, userManager.GetJWTClaims(quser));

        return resToken;
    }

    public async Task<RefreshTokenNfo?> RenewRefreshTokenAsync(
        string currentRefreshToken, CancellationToken cancellationToken)
    {
        var username = await IsRefreshTokenValidAsync(currentRefreshToken, cancellationToken);

        if (username is null) return null;

        return await GenerateRefreshTokenAsync(username, cancellationToken);
    }

    public async Task<bool> RemoveRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var res = false;
        var q = dbContext.UserRefreshTokens.FirstOrDefault(w => w.RefreshToken == refreshToken);

        if (q is not null)
        {
            res = true;

            await semRefreshToken.WaitAsync(cancellationToken);
            try
            {
                dbContext.Remove(q);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                semRefreshToken.Release();
            }
        }
        return res;
    }

}