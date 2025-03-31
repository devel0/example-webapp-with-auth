namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.DTOs;

/// <summary>
/// <see cref="AuthController.RenewRefreshToken"/> response api specific status.
/// </summary>
public enum RenewRefreshTokenStatus
{
    /// <summary>
    /// Authentication valid.
    /// </summary>        
    OK,

    /// <summary>
    /// Invalid authentication.
    /// </summary>        
    InvalidAuthentication,

    /// <summary>
    /// Invalid http context.
    /// </summary>        
    InvalidHttpContext,

    /// <summary>
    /// Access token not found.
    /// </summary>        
    AccessTokenNotFound,

    /// <summary>
    /// Invalid refresh token.
    /// </summary>        
    InvalidRefreshToken,
}


/// <summary>
/// <see cref="AuthController.RenewRefreshToken"/> api response data.
/// </summary>
public class RenewRefreshTokenResponse
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    [Required]
    public required RenewRefreshTokenStatus Status { get; set; }

    public RefreshTokenNfo? RefreshTokenNfo { get; set; }

}