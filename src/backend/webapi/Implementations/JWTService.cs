namespace ExampleWebApp.Backend.WebApi;

public class JWTService : IJWTService
{
    readonly ILogger<JWTService> logger;
    readonly IConfiguration configuration;
    readonly UserManager<ApplicationUser> userManager;
    readonly AppDbContext dbContext;

    static SemaphoreSlim semRefreshToken = new SemaphoreSlim(1);

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

    public TimeSpan AccessTokenLifetime => configuration.AppConfig().Auth.Jwt.AccessTokenDuration;

    public TimeSpan RefreshTokenLifetime => configuration.AppConfig().Auth.Jwt.RefreshTokenDuration;

    public SymmetricSecurityKey JwtEncryptionKey =>
        new SymmetricSecurityKey(Convert.FromBase64String(configuration.AppConfig().Auth.Jwt.Key));

    public string Issuer => configuration.AppConfig().Auth.Jwt.Issuer;

    public string Audience => configuration.AppConfig().Auth.Jwt.Audience;

    public string GenerateAccessToken(string username, string email, IList<Claim> claims)
    {
        var key = JwtEncryptionKey;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(AccessTokenLifetime),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(JwtEncryptionKey, JWT_SecurityAlghoritm)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var accessToken = tokenHandler.WriteToken(token);

        // logger.LogDebug($"generated access token : {accessToken}");

        return accessToken;
    }

    public async Task<RenewAccessTokenNfo?> RenewAccessTokenAsync(
        string accessToken, string refreshToken, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal is null)
        {
            logger.LogTrace($"invalid token principal");
            return null; // invalid access token        
        }

        var claimsUsername = principal.GetUserInfoFromClaims().UserName;

        if (claimsUsername is null)
        {
            logger.LogTrace($"no username in claims");
            return null; // no username in claims        
        }

        if (!IsRefreshTokenStillValid(claimsUsername, refreshToken))
        {
            // logger.LogTrace($"refresh token not more valid");
            return null; // refresh token not found                
        }

        var quser = dbContext.Users.FirstOrDefault(w => w.UserName == claimsUsername);

        if (quser is null || quser.LockoutEnd > DateTime.UtcNow) return null; // user not exists any more or lockouted

        var email = quser.Email;
        if (email is null) throw new Exception("email not set");

        var resToken = GenerateAccessToken(claimsUsername, email, userManager.GetJWTClaims(quser));

        return new RenewAccessTokenNfo
        {
            AccessToken = resToken,
            UserName = claimsUsername,
            Principal = principal
        };
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

    public async Task<RefreshTokenNfo?> RenewRefreshTokenAsync(
        string userName, string currentRefreshToken, CancellationToken cancellationToken)
    {
        if (IsRefreshTokenStillValid(userName, currentRefreshToken))
            return await GenerateRefreshTokenAsync(userName, cancellationToken);

        return null;
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

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken)
    {
        ClaimsPrincipal? principal;

        try
        {
            var tokenValidationParameters = configuration.GetTokenVaildationParameters(validateLifetime: false);

            var tokenHandler = new JwtSecurityTokenHandler();
            principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwt || jwt.Header.Alg != JWT_SecurityAlghoritm)
                return null;
        }
        catch
        {
            // Microsoft.IdentityModel.Tokens.SecurityTokenSignatureKeyNotFoundException
            return null;
        }

        return principal;
    }

    public bool IsRefreshTokenStillValid(string userName, string refreshToken)
    {
        var qrefresh = dbContext.UserRefreshTokens
            .FirstOrDefault(w => w.UserName == userName && w.RefreshToken == refreshToken);

        if (qrefresh is null)
        {
            return false; // refresh token associated with given username not found        
        }

        var utcNow = DateTimeOffset.UtcNow;

        return qrefresh.Expires > utcNow;
    }


}