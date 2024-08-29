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

                string? msg = "";

                if (ex is not null)
                {

                    if (ex.InnerException is PostgresException psqlException)
                        msg = psqlException.Message.Lines().FirstOrDefault();

                    else
                        msg = ex.Message.Lines().FirstOrDefault();

                }

                var title = "Exception";
                var detail = msg;
                var type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"; // internal server error

                if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
                {
                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails =
                        {
                            Title = title,
                            Detail = detail,
                            Type = type
                        }
                    });
                }
            });
        });

        return res;
    }


}