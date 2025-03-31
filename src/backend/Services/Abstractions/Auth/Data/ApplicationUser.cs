namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.Data;

public class ApplicationUser : IdentityUser
{

    public bool Disabled { get; set; }

}
