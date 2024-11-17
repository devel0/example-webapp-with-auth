namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{  

    public static bool IsUnitTest(this IConfiguration configuration) => configuration.AppConfig().IsUnitTest;

    /// <summary>
    /// Performs sanity check on configuration variables.
    /// </summary>
    public static void SanityCheck(this IConfiguration configuration)
    {
        var appConfig = configuration.AppConfig();

        var accessTokenDurationSeconds = appConfig.Auth.Jwt.AccessTokenDuration;

        var refreshTokenDurationSeconds = appConfig.Auth.Jwt.RefreshTokenDuration;

        if (accessTokenDurationSeconds >= refreshTokenDurationSeconds)
            throw new Exception($"access token duration must less than refresh token duration");
    }

}