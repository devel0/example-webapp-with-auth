namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Edit user data
/// </summary>
public class EditUserRequestDto
{

    /// <summary>
    /// True for new user, false to change existing one.
    /// </summary>
    public required bool CreateNew { get; set; }

    /// <summary>
    /// User name.
    /// </summary>    
    public required string UserName { get; set; }

    /// <summary>
    /// User email.
    /// </summary>    
    public required string Email { get; set; }

    /// <summary>
    /// Non null password to change the password.
    /// </summary>    
    public string? ChangePassword { get; set; }

    /// <summary>
    /// Roles to set to the user.
    /// </summary>    
    public required IList<string> Roles { get; set; }

}
