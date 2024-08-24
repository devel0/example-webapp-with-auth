namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Configure db connection string and provider.
    /// </summary>    
    public static void ConfigureDatabase(this WebApplicationBuilder webApplicationBuilder)
    {
        var isUnitTest = webApplicationBuilder.Configuration.GetConfigVar<bool>(CONFIG_KEY_IsUnitTest);

        var connString = isUnitTest ?
            webApplicationBuilder.Configuration.GetConfigVar(CONFIG_KEY_UnitTestConnectionString) :
            webApplicationBuilder.Configuration.GetConfigVar(CONFIG_KEY_ConnectionString);        
            
        var provider = webApplicationBuilder.Configuration.GetConfigVar<ConfigValuesDbProvider>(CONFIG_KEY_DbProvider);

        //
        // normal config for db providers
        //
        webApplicationBuilder.Services.AddDbContext<AppDbContext>(options =>
        {
            switch (provider)
            {

                case ConfigValuesDbProvider.Postgres:
                    {
                        options.UseNpgsql(connString, x => x.MigrationsAssembly("AppDbMigrationsPsql"));
                    }
                    break;

                default: throw new NotImplementedException($"provider {provider} not implemented");
            }

            options
                .EnableSensitiveDataLogging();

        });
    }

    /// <summary>
    /// Auto apply database pending migrations.
    /// </summary>    
    public static async Task ApplyDatabaseMigrations(this WebApplication app,
        CancellationToken cancellationToken)
    {
        using var scope = app.Services.CreateScope();
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var appliedMigrations = (await db.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();
            var pendingMigrations = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (appliedMigrations.Count == 0 && pendingMigrations.Count == 0)
                throw new Exception("no initial migration found");

            if (pendingMigrations.Count > 0)
            {
                app.Logger.LogInformation("database migrations");

                var migrator = db.Database.GetInfrastructure().GetService<IMigrator>();

                if (migrator is null)
                    throw new Exception($"unable to retrieve db migrator");

                foreach (var migration in pendingMigrations)
                {
                    await migrator.MigrateAsync(migration, cancellationToken);
                }
            }
        }
    }

}