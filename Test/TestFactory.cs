namespace Test;

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
    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        // retrieve configuration before start web app factory in order to drop test db
        {
            var builder = new ConfigurationBuilder();

            builder.SetupAppSettings("Development");

            var config = builder.Build();

            UnitTestConnectionString = config.GetConfigVar<string>(CONFIG_KEY_UnitTestConnectionString);

            UnitTestDbName = UnitTestConnectionString.Split(';')
                .Select(w => w.Trim())
                .First(w => w.StartsWith("Database="))
                .StripBegin("Database=");

            await DropDbAsync(cancellationToken);
        }

        // setting this config var cause the connection string database to another given name
        Environment.SetEnvironmentVariable(CONFIG_KEY_IsUnitTest.Replace(":", "__"), "true");        

        Factory = new WebApplicationFactory<Program>();

        Client = Factory.CreateClient();

        ServiceScope = Factory.Services.CreateScope();
    }

    async Task DropDbAsync(CancellationToken cancellationToken = default)
    {
        var ss = UnitTestConnectionString.Split(';');
        var NoDbConnectionString = string.Join(';', ss
            .Where(r => r.Trim().Length > 0 && !r.Trim().StartsWith("Database", StringComparison.InvariantCultureIgnoreCase)));

        await using var dataSource = NpgsqlDataSource.Create(NoDbConnectionString);

        await using var cmd = dataSource.CreateCommand($"DROP DATABASE IF EXISTS \"{UnitTestDbName}\" WITH (FORCE)");

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public void Dispose()
    {
        Factory.Dispose();
    }

}