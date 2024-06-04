namespace ExampleWebApp.Backend.Data;

public partial class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions options)
        : base(options)
    {

    }    

    public required DbSet<UserRefreshToken> UserRefreshTokens { get; set; }    

}
