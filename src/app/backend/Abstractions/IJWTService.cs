namespace ExampleWebApp.Backend.WebApi;

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
    /// or the invalidation of refresh tokens following a password reset (pwd reset not yet implemented).
    /// </summary>    
    TimeSpan AccessTokenLifetime { get; }

    /// <summary>
    /// Duration of refresh token.<br/>      
    /// Keep as short as possible. (default: 20min)<br/>
    /// When the access token expires and refresh token is still valid then a new refresh token is generated
    /// with a renewd refrsh token duration from that start time; this mean the session can continue
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
    /// Generate access token for given username, email, claims with duration from now plus <see cref="AccessTokenLifetime"/>.
    /// </summary>    
    string GenerateAccessToken(string username, string email, IList<Claim> claims);

    /// <summary>
    /// Renew given access token when it contains a principal not locked out 
    /// within related given refresh token not yet expired.    
    /// </summary>
    /// <param name="accessToken">Access token ( even if its expired ).</param>
    /// <param name="refreshToken">Valid Refresh Token.</param>    
    /// <returns>Null if refresh token isn't valid and a new login required.</returns>
    Task<RenewAccessTokenNfo?> RenewAccessTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Create a new refresh token for given username. Then db savechanges will applied.
    /// </summary>
    /// <param name="userName">Username which associate a new refresh token.</param>
    /// <returns>New valid refresh token associated to the given username.</returns>
    Task<RefreshTokenNfo> GenerateRefreshTokenAsync(string userName, CancellationToken cancellationToken);

    /// <summary>
    /// Purges expired and rotated+skew expired refresh tokens.
    /// </summary>
    /// <param name="userName">Username which purge refresh tokens.</param>
    Task MaintenanceRefreshTokenAsync(string userName, CancellationToken cancellationToken);

    /// <summary>
    /// Rotate given refresh token.
    /// </summary>    
    /// <param name="userName">Username associated to the given refresh token to rotate.</param>
    /// <param name="refreshTokenToRotate">Refresh token to rotate.</param>
    Task<string?> RotateRefreshTokenAsync(string userName, string refreshTokenToRotate, CancellationToken cancellationToken);

    /// <summary>
    /// Renew refresh token from given username and valid refresh token.
    /// </summary>
    Task<RefreshTokenNfo?> RenewRefreshTokenAsync(string userName, string currentRefreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Remove given refresh token from db.
    /// </summary>
    /// <param name="refreshToken">Refresh token to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if was found.</returns>
    Task<bool> RemoveRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve principal from accessToken even its expired.
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);

    /// <summary>
    /// States if given refreshToken is still valid ( exists and not yet expired ).
    /// </summary>    
    bool IsRefreshTokenStillValid(string userName, string refreshToken);

}