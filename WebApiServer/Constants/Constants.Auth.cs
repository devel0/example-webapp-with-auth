namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    public const string APP_CORS_POLICY_NAME = "MyCors";

    public const string JWT_SecurityAlghoritm = SecurityAlgorithms.HmacSha256;

    public const string ROLE_admin = "admin";
    public const string ROLE_user = "user";
    public const string ROLE_advanced = "advanced";

    /// <summary>
    /// user roles
    /// </summary>    
    public static readonly string[] ROLES_ALL = new[]
    {
        ROLE_admin,
        ROLE_user,
        ROLE_advanced
    };

}