namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.Login(LoginRequestDto)"/> response api specific status.
/// </summary>
public enum LoginStatus
{

    /// <summary>
    /// Login vaild.
    /// </summary>
    OK,

    /// <summary>
    /// Missing username or email.
    /// </summary>
    UsernameOrEmailRequired,

    /// <summary>
    /// Invalid authentication.
    /// </summary>    
    InvalidAuthentication,    

    /// <summary>
    /// Authentication http context.
    /// </summary>    
    InvalidHttpContext
}

/// <summary>
/// <see cref="AuthController.Login(LoginRequestDto)"/> api response data.
/// </summary>
public class LoginResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    public required LoginStatus Status { get; set; }

    /// <summary>
    /// Username.
    /// </summary>    
    public string UserName { get; set; } = "";

    /// <summary>
    /// Email.
    /// </summary>    
    public string Email { get; set; } = "";

    /// <summary>
    /// User roles.
    /// </summary>
    public List<string> Roles { get; set; } = new List<string>();

    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Permissions related to this user roles.
    /// </summary>
    public HashSet<UserPermission> Permissions { get; set; } = new();

}
