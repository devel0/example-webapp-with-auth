namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// setup mapping for serving spa static files
    /// </summary>
    public static void MapSpaStaticFiles(this WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            var spaPath = "/app";

            app.Map(new PathString(spaPath), client =>
            {
                client.UseSpaStaticFiles();
                client.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "clientapp";
                    spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                    {
                        OnPrepareResponse = ctx =>
                        {
                            var headers = ctx.Context.Response.GetTypedHeaders();
                            headers.CacheControl = new CacheControlHeaderValue
                            {
                                NoCache = true,
                                NoStore = true,
                                MustRevalidate = true
                            };
                        }
                    };
                });
            });
        }
    }
}