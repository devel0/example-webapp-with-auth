namespace ExampleWebApp.Backend.WebApi.Types;

public class UsernameAuthOptions
{

     public required string AllowedUserNameCharacters { get; set; }

}

public class PasswordAuthOptions
{
    public required bool RequireDigit { get; set; }

    public required int RequiredLength { get; set; }

    public required int RequiredUniqueChars { get; set; }

    public required bool RequireLowercase { get; set; }

    public required bool RequireNonAlphanumeric { get; set; }

    public required bool RequireUppercase { get; set; }
}

public class AuthOptions
{

    public required UsernameAuthOptions Username { get; set; }

    public required PasswordAuthOptions Password { get; set; }

}