namespace ExampleWebApp.Backend.Data.Types;

public class ApplicationUser : IdentityUser
{

    public bool Disabled { get; set; }

}
