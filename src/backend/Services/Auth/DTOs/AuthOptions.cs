namespace ExampleWebApp.Backend.WebApi.Services.Auth.DTOs;

public class UsernameAuthOptions
{

    [Required]
    public required string AllowedUserNameCharacters { get; set; }

}

public class PasswordAuthOptions
{
    [Required]
    public required bool RequireDigit { get; set; }

    [Required]
    public required int RequiredLength { get; set; }

    [Required]
    public required int RequiredUniqueChars { get; set; }

    [Required]
    public required bool RequireLowercase { get; set; }

    [Required]
    public required bool RequireNonAlphanumeric { get; set; }

    [Required]
    public required bool RequireUppercase { get; set; }
}

public class AuthOptions
{

    public required UsernameAuthOptions Username { get; set; }

    public required PasswordAuthOptions Password { get; set; }

}