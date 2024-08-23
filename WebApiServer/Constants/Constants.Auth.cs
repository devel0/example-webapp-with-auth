namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    public const int PASSWORD_MIN_LENGTH = 8;

    public static readonly TimeSpan LOGIN_FAIL_LOCKOUT_DURATION = TimeSpan.FromMinutes(5);

    /// <summary>
    /// after <see cref="MAX_LOGIN_FAIL_ATTEMPTS"/> then user will locked out for <see cref="LOGIN_FAIL_LOCKOUT_DURATION"/> 
    /// </summary>
    public const int MAX_LOGIN_FAIL_ATTEMPTS = 5;

    public const string APP_CORS_POLICY_NAME = "MyCors";

    public const string JWT_SecurityAlghoritm = SecurityAlgorithms.HmacSha256;

    /// <summary>
    /// can create and edit username, email, password, roles
    /// </summary>
    public const string ROLE_admin = "admin";

    /// <summary>
    /// can edit its own email, password;
    /// can create and edit username, email, password for other users that are only in the "normal" role
    /// </summary>
    public const string ROLE_advanced = "advanced";

    /// <summary>
    /// can edit its own email, password
    /// </summary>
    public const string ROLE_normal = "normal";

    /// <summary>
    /// user roles
    /// </summary>    
    public static readonly string[] ROLES_ALL = new[]
    {
        ROLE_admin,
        ROLE_normal,
        ROLE_advanced
    };

}