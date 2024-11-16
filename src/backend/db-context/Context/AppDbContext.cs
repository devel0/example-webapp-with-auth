namespace ExampleWebApp.Backend.Data;

public partial class AppDbContext : IdentityDbContext<ApplicationUser>
{
    readonly IConfiguration configuration;

    public AppDbContext(
        DbContextOptions options,
        IConfiguration configuration
        )
        : base(options)
    {
        this.configuration = configuration;
    }

    public required DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var useSnakeCaseMode = configuration.GetConfigVar<bool>(CONFIG_KEY_DbSchemaSnakeCase);

        if (useSnakeCaseMode == true)
        {

            var ents = builder.Model.GetEntityTypes().ToList();

            foreach (var entity in ents)
            {
                entity.SnakeCaseMode();
            }

        }
    }

}
