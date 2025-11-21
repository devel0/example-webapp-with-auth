namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Configure db connection string and provider.
    /// </summary>    
    public static void ConfigureDatabase(this WebApplicationBuilder webApplicationBuilder)
    {
        var appConfig = webApplicationBuilder.Configuration.GetAppConfig();

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
                        options.UseNpgsql(connString, options =>
                        {
                            var migrationAssembly = Assembly.GetAssembly(typeof(AppDbContext));
                            if (migrationAssembly is null) throw new Exception($"couldn't find migration assembly");
                            options.MigrationsAssembly(migrationAssembly.FullName);
                        });
                    }
                    break;

                case AppConfig.DatabaseConfig.ConnectionItemConfig.DbProviderConfig.Mysql:
                    {
                        var serverVersion = ServerVersion.AutoDetect(connString);

                        options.UseMySql(connString, serverVersion, options =>
                        {
                            var migrationAssembly = Assembly.GetAssembly(typeof(AppDbContext));
                            if (migrationAssembly is null) throw new Exception($"couldn't find migration assembly");
                            options.MigrationsAssembly(migrationAssembly.FullName);
                        });
                    }
                    break;

                default: throw new NotImplementedException($"provider {provider} not implemented");
            }

            if (!isUnitTest)
                options.EnableSensitiveDataLogging();

            if (isUnitTest)
                options.EnableServiceProviderCaching(false);

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

            var appConfig = app.Configuration.GetAppConfig();

            // if (app.Configuration.IsUnitTest())
            //     System.Console.WriteLine("============> APPLY MIGRATION");

            var appliedMigrations = (await db.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();

            var pendingMigrations = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

            if (appliedMigrations.Count == 0 && pendingMigrations.Count == 0)
                throw new Exception("no initial migration found");

            if (appConfig.DatabaseProvider == DbProviderConfig.Postgres)
            {
                //
                // create functions
                //
                {
                    #region GuidString
                    {
                        var sql =
                            $"""
                            create or replace function "{DBFN_GUID_STRING}"(field uuid)
                            returns text as
                            $$
                            select field::text
                            $$
                            language sql
                            """;

                        await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                    }
                    #endregion
                }
            }

            if (pendingMigrations.Count > 0)
            {
                app.Logger.LogInformation("database migrations");

                var migrator = db.Database.GetInfrastructure().GetService<IMigrator>();

                db.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

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