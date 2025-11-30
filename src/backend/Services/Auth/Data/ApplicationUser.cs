namespace ExampleWebApp.Backend.WebApi.Services.Auth.Data;

public class ApplicationUser : IdentityUser
{

    public bool Disabled { get; set; }

}
