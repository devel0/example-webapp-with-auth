namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    public const string API_BASE_URL = "/api";

    // Swagger    
    public static string SWAGGER_CSS_PATH() =>
         Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Misc", "SwaggerDark.css");
    public const string SWAGGER_API_TITLE = "ExampleWebApp API";

    // api STATUS

    public const int STATUS_OK = 0;
    public const int STATUS_InternalError = 1;
    public const int STATUS_InvalidArgument = 2;
    public const int STATUS_InvalidAuthentication = 3;
    public const int STATUS_Unknown = 4;
    public const int STATUS_IdentityError = 5;
    public const int STATUS_AdminRolesReadOnly = 6;
    public const int STATUS_UserNotFound = 7;
    public const int STATUS_InvalidHttpContext = 8;
    public const int STATUS_InvalidPassword= 9;

    public const int STATUS_Custom = 100;

}