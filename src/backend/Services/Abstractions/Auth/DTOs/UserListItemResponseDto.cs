namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.DTOs;

/// <summary>
/// <see cref="AuthController.ListUsers"/> api response data.
/// </summary>
public class UserListItemResponseDto
{

    /// <summary>
    /// User name.
    /// </summary>
    [Required]
    public required string UserName { get; set; }

    /// <summary>
    /// User email.
    /// </summary>
    [Required]
    public required string Email { get; set; }

    /// <summary>
    /// User roles.
    /// </summary>
    [Required]
    public required IList<string> Roles { get; set; }

    [Required]
    public required bool Disabled { get; set; }

    /// <summary>
    /// Access failed count.
    /// </summary>    
    [Required]
    public required int AccessFailedCount { get; set; }

    /// <summary>
    /// Email is confirmed.
    /// </summary>    
    [Required]
    public required bool EmailConfirmed { get; set; }

    /// <summary>
    /// Lockout end (UTC). <see cref="LockoutEnabled"/>.
    /// </summary>
    [Required]
    public required DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// If true the user is lockout until <see cref="LockoutEnd"/>.
    /// </summary>
    [Required]
    public required bool LockoutEnabled { get; set; }

    /// <summary>
    /// User phone number.
    /// </summary>    
    [Required]
    public required string? PhoneNumber { get; set; }

    /// <summary>
    /// User phone number confirmed.
    /// </summary>    
    [Required]
    public required bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Two factor enabled.
    /// </summary>    
    [Required]
    public required bool TwoFactorEnabled { get; set; }

}
