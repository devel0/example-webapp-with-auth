namespace ExampleWebApp.Backend.WebApi.Data;

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

        // TODO: x database config
        var useSnakeCaseMode = configuration.GetAppConfig().Database.SchemaSnakeCase;

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
