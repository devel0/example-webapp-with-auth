namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// custommize exception handling
    /// </summary>
    public static IApplicationBuilder SetupException(this WebApplication app)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-7.0
        var res = app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = Text.Plain;

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                var ex = exceptionHandlerPathFeature?.Error;

                if (ex?.InnerException is PostgresException psqlException)
                {
                    var msg = psqlException.Message?.Lines().FirstOrDefault();
                    await context.Response.WriteAsync(msg ?? "db exception");
                }

                else
                {
                    var msg = ex.Message?.Lines().FirstOrDefault();
                    await context.Response.WriteAsync(msg ?? "internal error");
                }
            });
        });

        return res;
    }


}