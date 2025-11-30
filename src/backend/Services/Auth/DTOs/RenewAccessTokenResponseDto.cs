namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.DTOs;

/// <summary>
/// <see cref="AuthController.RenewAccessToken"/> response api specific status.
/// </summary>
public enum RenewAccessTokenStatus
{
    /// <summary>
    /// Valid refresh token, thus access token renewd.
    /// </summary>        
    OK,

    /// <summary>
    /// Invalid authentication.
    /// </summary>        
    InvalidAuthentication,

    /// <summary>
    /// Invalid access token.
    /// </summary>        
    InvalidAccessToken,

    /// <summary>
    /// Invalid refresh token.
    /// </summary>        
    InvalidRefreshToken,

    /// <summary>
    /// Invalid http context.
    /// </summary>        
    InvalidHttpContext,
}

/// <summary>
/// <see cref="AuthController.RenewAccessToken"/> api response data.
/// </summary>
public class RenewAccessTokenResponse
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    [Required]
    public required RenewAccessTokenStatus Status { get; set; }

    public AccessTokenNfo? AccessTokenNfo { get; set; }

}