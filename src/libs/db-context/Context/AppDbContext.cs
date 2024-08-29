namespace ExampleWebApp.Backend.Data;

public partial class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {

    }    

    public required DbSet<UserRefreshToken> UserRefreshTokens { get; set; }    

}
