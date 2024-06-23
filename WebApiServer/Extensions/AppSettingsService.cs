namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// setup loading of configuration from appsettings.json (mandatory), appsettings.[Environment].json (optional)
    /// with autoreload on change ; add configuration from either user-secrets (development) and
    /// from environment variables ( replacing : with __ )
    /// </summary>    
    public static void SetupAppSettings(this WebApplicationBuilder builder)
    {
        var appsettingsBase = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "appsettings.json");

        var appsettingsEnv = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            $"appsettings.{builder.Environment.EnvironmentName}.json");

        builder.Configuration
            .AddJsonFile(
                appsettingsBase,
                optional: false,
                reloadOnChange: true)

            .AddJsonFile(
                appsettingsEnv,
                optional: true,
                reloadOnChange: true)

            .AddEnvironmentVariables()

            .AddUserSecrets(Assembly.GetExecutingAssembly())
            ;
    }

}