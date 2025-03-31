namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.DTOs;

/// <summary>
/// Username and email claims info.
/// </summary>
public class UserClaimsNfo
{

    /// <summary>
    /// User name.
    /// </summary>    
    public string? UserName { get; set; }

    /// <summary>
    /// Email.
    /// </summary>    
    public string? Email { get; set; }
    
}
