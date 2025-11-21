namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    public const string API_PREFIX = "/api";

    // Swagger    
    public static string SWAGGER_CSS_PATH() =>
         Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Misc", "SwaggerDark.css");
    public const string SWAGGER_API_TITLE = "ExampleWebApp API";   

    public static readonly TimeSpan WS_KEEPALIVE_INTERVAL = TimeSpan.FromSeconds(30);

}