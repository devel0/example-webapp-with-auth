namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Configure db connection string and provider.
    /// </summary>    
    public static void ConfigureDatabase(this WebApplicationBuilder webApplicationBuilder)
    {
        var appConfig = webApplicationBuilder.Configuration.AppConfig();

        var isUnitTest = appConfig.IsUnitTest;

        var connectionName = isUnitTest ? UNIT_TEST_DB_CONN_NAME : appConfig.Database.ConnectionName;

        var qDbConfig = appConfig.Database.Connections.FirstOrDefault(w => w.Name == connectionName);

        if (qDbConfig is null)
            throw new Exception($"Can't find suitable connection named \"{connectionName}\" in configuration");

        var connString = qDbConfig.ConnectionString;

        var provider = qDbConfig.Provider;

        //
        // normal config for db providers
        //
        webApplicationBuilder.Services.AddDbContext<AppDbContext>(options =>
        {
            switch (provider)
            {

                case AppConfig.DatabaseConfig.ConnectionItemConfig.DbProviderConfig.Postgres:
                    {
                        options.UseNpgsql(connString, x => x.MigrationsAssembly("db-migrations-psql"));
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

            //
            // dev note: if not found migration assembly, double check the list of assemblies
            // see below commented sample dump
            //
            // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // foreach (var assembly in assemblies)
            // {
            //     System.Console.WriteLine($"Assembly [{assembly.GetName()}]");
            // }
            //

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