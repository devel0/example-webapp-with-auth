namespace ExampleWebApp.Backend.WebApi.Services.Auth.DTOs;

/// <summary>
/// <see cref="AuthController.Login(LoginRequestDto)"/> api request data.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Username or email.
    /// </summary>    
    public string? UsernameOrEmail { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    public required string Password { get; set; }

    public string? PasswordResetToken { get; set; }
}
