namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.DTOs;

/// <summary>
/// Edit user data
/// </summary>
public class EditUserRequestDto
{

    /// <summary>
    /// Existing User name or null to create a new one using <see cref="EditUsername"/>.
    /// </summary>    
    public string? ExistingUsername { get; set; }

    /// <summary>
    /// New email or null to leave unchanged.
    /// </summary>    
    public string? EditEmail { get; set; }

    /// <summary>
    /// New username or null to leave unchanged.
    /// </summary>
    public string? EditUsername { get; set; }

    /// <summary>
    /// New password or null to leave unchanged.
    /// </summary>    
    public string? EditPassword { get; set; }

    /// <summary>
    /// Roles to set to the user or null to leave unchanged.
    /// </summary>    
    public IList<string>? EditRoles { get; set; }

    /// <summary>
    /// If true the user can't login after previous release access token expires.
    /// </summary>
    public bool? EditDisabled { get; set; }

    /// <summary>
    /// Set the end date of lockout.
    /// The user will be unable to login until <see cref="EditLockoutEnd"/>.
    /// If <see cref="EditLockoutEnd"/> is set in the past the user will be re-enabled immediately.
    /// </summary>
    public DateTimeOffset? EditLockoutEnd { get; set; }

}
