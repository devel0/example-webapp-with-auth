namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// setup client static files serving on production
    /// </summary>
    public static void SetupSpa(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            builder.Services.AddSpaStaticFiles(configuration =>
            {
                var clientRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clientapp");                
                configuration.RootPath = clientRootPath;
            });
        }
    }

}