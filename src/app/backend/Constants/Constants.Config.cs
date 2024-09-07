namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{


    public enum ConfigValuesDbProvider
    {
        Postgres
    };

    public enum ConfigValuesEmailServerSecurity
    {
        None,
        Auto,
        Ssl,
        Tls
    }

    //---

    public const string CONFIG_KEY_AppServerName = "AppServerName";
    public const string CONFIG_KEY_ConnectionString = "ConnectionStrings:Main";
    public const string CONFIG_KEY_DbProvider = "DbProvider";

    public const string CONFIG_KEY_IsUnitTest = "IsUnitTest";
    public const string CONFIG_KEY_UnitTestConnectionString = "ConnectionStrings:UnitTest";

    public const string CONFIG_KEY_JwtSettings_SECTION = "JwtSettings";
    public const string CONFIG_KEY_JwtSettings_Key = "JwtSettings:Key";
    public const string CONFIG_KEY_JwtSettings_Issuer = "JwtSettings:Issuer";
    public const string CONFIG_KEY_JwtSettings_Audience = "JwtSettings:Audience";

    /// <summary>
    /// duration of access token
    /// </summary>
    public const string CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds = "JwtSettings:AccessTokenDurationSeconds";

    /// <summary>
    /// duration of refresh token
    /// </summary>
    public const string CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds = "JwtSettings:RefreshTokenDurationSeconds";

    /// <summary>
    /// post-duration of refresh token after rotation
    /// </summary>
    public const string CONFIG_KEY_JwtSettings_RefreshTokenRotationSkewSeconds = "JwtSettings:RefreshTokenDurationSkewSeconds";

    public const string CONFIG_KEY_JwtSettings_ClockSkewSeconds = "JwtSettings:ClockSkewSeconds";

    public const string CONFIG_KEY_SeedUsers_Admin_UserName = "SeedUsers:Admin:UserName";
    public const string CONFIG_KEY_SeedUsers_Admin_Password = "SeedUsers:Admin:Password";
    public const string CONFIG_KEY_SeedUsers_Admin_Email = "SeedUsers:Admin:Email";

    public const string CONFIG_KEY_EmailServer_Username = "EmailServer:Username";
    public const string CONFIG_KEY_EmailServer_Password = "EmailServer:Password";
    public const string CONFIG_KEY_EmailServer_SmtpServer = "EmailServer:SmtpServerName";
    public const string CONFIG_KEY_EmailServer_SmtpServerPort = "EmailServer:SmtpServerPort";
    public const string CONFIG_KEY_EmailServer_Security = "EmailServer:Security";
    public const string CONFIG_KEY_EmailServer_FromDisplayName = "EmailServer:FromDisplayName";



}