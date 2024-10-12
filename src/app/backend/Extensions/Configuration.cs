namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{
   
    public static bool IsUnitTest(this IConfiguration configuration) =>
        configuration.GetConfigVar<bool>(CONFIG_KEY_IsUnitTest);

    /// <summary>
    /// Performs sanity check on configuration variables.
    /// </summary>
    public static void SanityCheck(this IConfiguration configuration)
    {
        var accessTokenDurationSeconds =
            configuration.GetConfigVar<double>(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds);

        var refreshTokenDurationSeconds =
            configuration.GetConfigVar<double>(CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds);

        if (accessTokenDurationSeconds >= refreshTokenDurationSeconds)
            throw new Exception($"{CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds} must less than {CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds}");

        var refreshTokenDurationSkewSeconds =
            configuration.GetConfigVar<double>(CONFIG_KEY_JwtSettings_RefreshTokenRotationSkewSeconds);

        if (refreshTokenDurationSkewSeconds >= refreshTokenDurationSeconds)
            throw new Exception($"{CONFIG_KEY_JwtSettings_RefreshTokenRotationSkewSeconds} must less than {CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds}");        
    }

}