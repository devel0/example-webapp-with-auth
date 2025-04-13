namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth;

public record AccessTokenNfo(string AccessToken, DateTimeOffset Expiration);

public record RefreshTokenNfo(string RefreshToken, DateTimeOffset Expiration);

/// <summary>
/// JWT Service helper.
/// </summary>
public interface IJWTService
{

    /// <summary>
    /// Duration of generated access token.<br/>    
    /// Keep as short as possible. (default: 5min)
    /// Small duration on access token makes effective the activation of the lockout of the user
    /// or the invalidation of refresh tokens following a password reset.
    /// </summary>    
    TimeSpan AccessTokenLifetime { get; }

    /// <summary>
    /// Duration of refresh token.<br/>      
    /// Keep as short as possible. (default: 20min)<br/>
    /// When the access token expires and refresh token is still valid then a new refresh token is generated
    /// with a renewd refresh token duration from that start time; this mean the session can continue
    /// without further authentication even if the refresh token have a small duration.<br/>
    /// Small duration on refresh token reduce risk of crfs attack because when refresh token expires
    /// the only way to authorize a new request is to authenticate again through login.<br/>    
    /// </summary>    
    TimeSpan RefreshTokenLifetime { get; }

    /// <summary>
    /// JWT encryption key.
    /// </summary>    
    SymmetricSecurityKey JwtEncryptionKey { get; }

    /// <summary>
    /// Issuer identifier.
    /// </summary>    
    string Issuer { get; }

    /// <summary>
    /// Application identifier.
    /// </summary>    
    string Audience { get; }

    /// <summary>
    /// Extract token parts.
    /// </summary>
    JwtSecurityToken? DecodeAccessToken(string accessToken);   

    /// <summary>
    /// Add given access token to the http response as set cookie httponly, secure, samesite strict
    /// </summary>
    void AddAccessTokenToHttpResponse(HttpResponse response, AccessTokenNfo accessTokenNfo);

    /// <summary>
    /// Add given refresh token to the http response as set cookie httponly, secure, samesite strict
    /// </summary>
    void AddRefreshTokenToHttpResponse(HttpResponse response, RefreshTokenNfo refreshTokenNfo);

    /// <summary>
    /// Retrieve access token from the http request cookie. null if not was found.
    /// </summary>
    string? GetAccessTokenFromHttpRequest(HttpRequest request);

    /// <summary>
    /// Retrieve refresh token from the http request cookie. null if not was found.
    /// </summary>
    string? GetRefreshTokenFromHttpRequest(HttpRequest request);

    /// <summary>
    /// Set access token to be removed to the http response.
    /// </summary>
    void DeleteAccessTokenFromReponse(HttpResponse response);

    /// <summary>
    /// Set refresh token to be removed to the http response.
    /// </summary>
    void DeleteRefreshTokenFromReponse(HttpResponse response);

    /// <summary>
    /// States if token still valid.
    /// </summary>
    bool IsAccessTokenValid(string accessToken);

    /// <summary>
    /// States if given refreshToken is still valid ( exists and not yet expired ).
    /// </summary>    
    /// <returns>username associated with refresh token</returns>
    Task<string?> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Generate access token for given username, email, claims with duration from now plus <see cref="AccessTokenLifetime"/>.
    /// </summary>    
    AccessTokenNfo GenerateAccessToken(string username, string email, IList<Claim> claims);    

    /// <summary>
    /// Create a new refresh token for given username. Then db savechanges will applied.
    /// </summary>
    /// <param name="userName">Username which associate a new refresh token.</param>
    /// <returns>New valid refresh token associated to the given username.</returns>
    Task<RefreshTokenNfo> GenerateRefreshTokenAsync(string userName, CancellationToken cancellationToken);

    /// <summary>
    /// Purges expired refresh tokens.
    /// </summary>
    /// <param name="userName">Username which purge refresh tokens.</param>
    Task MaintenanceRefreshTokenAsync(string userName, CancellationToken cancellationToken);

    /// <summary>
    /// Generate new valid access token when refresh token still valid and user not disabled or lockedout.    
    /// </summary>    
    /// <param name="refreshToken">Valid Refresh Token.</param>    
    /// <returns>Null if current refresh token not valid or user disabled/lockedout.</returns>
    Task<AccessTokenNfo?> RenewAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Generate new valid refresh token when refresh token still valid.
    /// </summary>
    Task<RefreshTokenNfo?> RenewRefreshTokenAsync(string currentRefreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Remove given refresh token from db.
    /// </summary>
    /// <param name="refreshToken">Refresh token to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if was found.</returns>
    Task<bool> RemoveRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);    

}