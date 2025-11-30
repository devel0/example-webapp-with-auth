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

    public required DbSet<FakeData> FakeDatas { get; set; }

    void ConfigureDbFunctions(ModelBuilder builder)
    {
        var appConfig = configuration.GetAppConfig();

        if (appConfig.DatabaseProvider == DbProviderConfig.Postgres)
        {
            var method = typeof(DbFun).GetMethod(nameof(DbFun.GuidString));
            if (method is not null)
                builder
                    .HasDbFunction(method)
                    .HasName(DBFN_GUID_STRING)
                    .HasParameter("guid").HasStoreType("uuid");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureDbFunctions(builder);

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
