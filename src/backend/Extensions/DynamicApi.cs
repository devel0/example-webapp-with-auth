namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    public static void ConfigApis(this WebApplication app)
    {
        var tags = new[] { "Admin" };

        // ----------------------------- admin

        // var apiUrl = $"{API_BASE_URL}/LongRunning";

        // app.MapGet(apiUrl, async (
        //     [FromServices] CancellationToken cancellationToken) =>
        // {
        //     await Task.Delay(1000, cancellationToken);

        //     return Results.Ok();
        // })
        // .RequireAuthorization(ROLE_admin)
        // .WithMetadata(new SwaggerOperationAttribute
        // {
        //     Summary = $"Long running task.",
        //     Tags = tags,
        //     OperationId = apiUrl
        // });
        // .WithOpenApi();

    }

}
