namespace ExampleWebApp.Backend.WebApi.Services.Auth.DTOs;

/// <summary>
/// <see cref="AuthController.CurrentUser"/> response api specific status.
/// </summary>
public enum CurrentUserStatus
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
    /// Invalid argument.
    /// </summary>        
    InvalidArgument,

    /// <summary>
    /// Access token not found.
    /// </summary>        
    AccessTokenNotFound
}

/// <summary>
/// <see cref="AuthController.CurrentUser"/> api response data.
/// </summary>
public class CurrentUserResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    [Required]
    public required CurrentUserStatus Status { get; set; }

    /// <summary>
    /// Login username.
    /// </summary>    
    public string UserName { get; set; } = "";

    /// <summary>
    /// Email address.
    /// </summary>    
    public string Email { get; set; } = "";

    /// <summary>
    /// List of roles associated to this user.
    /// </summary>    
    public HashSet<string> Roles { get; set; } = new();

    /// <summary>
    /// Permissions related to this user roles.
    /// </summary>
    public HashSet<UserPermission> Permissions { get; set; } = new();

}
