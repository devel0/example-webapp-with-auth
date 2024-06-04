namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Connects host application lifetime started, stopping, stopped magaging cancellation of given token source
    /// </summary>
    public static IHostApplicationLifetime SetupLifetime(this WebApplication app, CancellationTokenSource cts)
    {

        var life = app.Services.GetRequiredService<IHostApplicationLifetime>();
        life.ApplicationStopping.Register(() =>
        {
            app.Logger.LogInformation("Backend application stopping");
            cts.Cancel();
        });
        life.ApplicationStarted.Register(() => app.Logger.LogInformation("Backend application started"));
        life.ApplicationStopped.Register(() => app.Logger.LogInformation("Backend application stopped"));

        return life;

    }

}