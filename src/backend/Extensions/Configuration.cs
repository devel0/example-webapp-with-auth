namespace ExampleWebApp.Backend.WebApi;

public class ReflectionHelper<T>
{
    public static string GetPath<TProperty>(Expression<Func<T, TProperty>> expr)
    {
        var name = expr.Parameters[0].Name;

        return expr.Body.ToString().Replace($"{name} => {name}", typeof(T).Name);
    }
}

public static partial class Extensions
{

    public static bool IsUnitTest(this IConfiguration configuration) => configuration.GetAppConfig().IsUnitTest;

    /// <summary>
    /// Performs sanity check on configuration variables.
    /// </summary>
    public static void SanityCheck(this IConfiguration configuration)
    {
        var appConfig = configuration.GetAppConfig();

        var accessTokenDurationSeconds = appConfig.Auth.Jwt.AccessTokenDuration;

        var refreshTokenDurationSeconds = appConfig.Auth.Jwt.RefreshTokenDuration;

        if (accessTokenDurationSeconds >= refreshTokenDurationSeconds)
            throw new Exception($"access token duration must less than refresh token duration");
    }

    public static void SetConfigVar(this IConfiguration configuration, string path, string value)
    {
        if (path.Contains(':'))
        {
            var ss = path.Split(':');
            var sectionPath = string.Join(':', ss.Take(ss.Length - 1));
            var section = configuration.GetRequiredSection(sectionPath);
            section[ss.Last()] = value;
        }
        else
            configuration[path] = value;
    }

    /// <summary>
    /// Helper to retrieve configuration from environment or appsettings if found.
    /// From the environment the same config key variable will be searched within colon replaced by underline.
    /// </summary>
    /// <param name="path">Variable path (ie. <see cref="CONFIG_KEY_AppConfig_IsUnitTest"/></param>    
    /// <param name="configuration">asp net configuration</param>    
    public static string? GetConfigVar(this IConfiguration configuration, string path) =>
        configuration.GetConfigVar<string?>(path);

    static bool IsReferenceOrNullableType(Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

    /// <summary>
    /// Typed helper to retrieve configuration from environment or appsettings if found.
    /// From the environment the same config key variable will be searched within colon replaced by underline.
    /// </summary>
    /// <param name="path">Variable path (ie. <see cref="CONFIG_KEY_AppConfig_IsUnitTest"/></param>    
    /// <param name="configuration">asp net configuration</param>    
    public static T? GetConfigVar<T>(this IConfiguration configuration, string path)
    {
        var type = typeof(T);

        if (!IsReferenceOrNullableType(type))
        {
            if (configuration.GetSection(path).Value is null)
                throw new Exception($"Unable to find value for config var [{path}]. Check your user secrets.");
        }

        var value = configuration.GetValue<T>(path);

        return value;
    }


    /// <summary>
    /// retrieve <see cref="AppConfig"/> from configuration
    /// </summary>
    public static AppConfig GetAppConfig(this IConfiguration configuration)
    {
        var appConfig = configuration.GetSection(CONFIG_KEY_AppConfig).Get<AppConfig>();

        if (appConfig is null)
            throw new Exception($"Can't load {nameof(AppConfig)} object from appsettings.json");

        return appConfig;
    }

    /// <summary>
    /// change a value in <see cref="AppConfig"/> reflecting to the configuration section
    /// </summary>
    public static void SetValue<P, T>(this AppConfig appConfig,
        IConfiguration configuration, Expression<Func<AppConfig, P>> path, T value) =>
        AppConfig.SetValue(configuration, path, value);

}