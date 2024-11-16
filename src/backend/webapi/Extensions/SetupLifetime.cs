namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Connects host application lifetime started, stopping, stopped magaging cancellation of given token source
    /// </summary>
    public static IHostApplicationLifetime SetupLifetime(this WebApplication app, CancellationTokenSource cts)
    {

        var life = app.Services.GetRequiredService<IHostApplicationLifetime>();
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        life.ApplicationStopping.Register(() =>
        {
            if (!configuration.IsUnitTest())
                app.Logger.LogInformation("Backend application stopping");
            cts.Cancel();
        });

        life.ApplicationStarted.Register(() =>
        {
            if (!configuration.IsUnitTest())
                app.Logger.LogInformation("Backend application started");
        });

        life.ApplicationStopped.Register(() =>
        {
            if (!configuration.IsUnitTest())
                app.Logger.LogInformation("Backend application stopped");
        });

        return life;

    }

}