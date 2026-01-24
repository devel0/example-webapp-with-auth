[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ExampleWebApp.Backend.Test;

public class TestFactory : IDisposable
{

    WebApplicationFactory<Program> Factory;

    string UnitTestConnectionString;
    string UnitTestDbName;

    public HttpClient Client;

    IServiceScope ServiceScope;

    public IServiceProvider Services => ServiceScope.ServiceProvider;

    public TestFactory()
    {
    }

    /// <summary>
    /// Initialize a test factory with IsUnitTest environment, test database dropped and service scope, http client allocted.
    /// </summary>
    public async Task<HttpClient> InitAsync(bool dropDb = true, CancellationToken cancellationToken = default)
    {
        // retrieve configuration before start web app factory in order to drop test db
        {
            Environment.SetEnvironmentVariable($"{nameof(AppConfig)}__{nameof(AppConfig.IsUnitTest)}", "true");

            var builder = new ConfigurationBuilder();

            builder.SetupAppSettings("Development");

            var config = builder.Build();

            var appConfig = config.GetAppConfig();

            var qDbConfig = appConfig.Database.Connections.FirstOrDefault(w => w.Name == UNIT_TEST_DB_CONN_NAME);

            if (qDbConfig is null)
            {
                throw new Exception($"Can't find suitable connection named \"{UNIT_TEST_DB_CONN_NAME}\" in configuration");
            }

            UnitTestConnectionString = qDbConfig.ConnectionString;

            UnitTestDbName = UnitTestConnectionString.Split(';')
                .Select(w => w.Trim())
                .First(w => w.StartsWith("Database="))
                .StripBegin("Database=");

            if (dropDb)
                await DropDbAsync(cancellationToken);
        }

        // setting this config var cause the connection string database to another given name
        // Environment.SetEnvironmentVariable(CONFIG_KEY_AppConfig_IsUnitTest.Replace(":", "__"), "true");

        Factory = new WebApplicationFactory<Program>();        

        Client = Factory.CreateClient();

        ServiceScope = Factory.Services.CreateScope();        

        return Client;
    }

    async Task DropDbAsync(CancellationToken cancellationToken = default)
    {
        var ss = UnitTestConnectionString.Split(';');
        var PostgresDbConnectionString = string.Join(';', ss
            .Where(r => r.Trim().Length > 0 && !r.Trim().StartsWith("Database", StringComparison.InvariantCultureIgnoreCase)))
            + "; Database=postgres";

        await using var dataSource = NpgsqlDataSource.Create(PostgresDbConnectionString);

        await using var cmd = dataSource.CreateCommand($"DROP DATABASE IF EXISTS \"{UnitTestDbName}\" WITH (FORCE)");        

        await cmd.ExecuteNonQueryAsync(cancellationToken);

        await using var cmd2 = dataSource.CreateCommand($"CREATE DATABASE \"{UnitTestDbName}\"");

        await cmd2.ExecuteNonQueryAsync(cancellationToken);
    }

    public void Dispose()
    {        
        Factory?.Dispose();
    }

}