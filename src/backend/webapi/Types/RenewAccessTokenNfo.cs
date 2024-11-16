namespace ExampleWebApp.Backend.WebApi.Types;

/// <summary>
/// <see cref="IJWTService.RenewAccessToken"/> result data.
/// </summary>
public class RenewAccessTokenNfo
{

    /// <summary>
    /// Access token.
    /// </summary>   
    [Required]
    public required string AccessToken { get; set; }    

    /// <summary>
    /// Username.
    /// </summary>    
    [Required]
    public required string UserName { get; set; }

    /// <summary>
    /// Claims principal.
    /// </summary>    
    [Required]
    public required ClaimsPrincipal Principal { get; set; }

}
