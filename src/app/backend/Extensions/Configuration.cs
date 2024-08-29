namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

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
    /// <param name="path">Variable path (ie. <see cref="CONFIG_KEY_JwtSettings_Key"/></param>    
    public static string GetConfigVar(this IConfiguration configuration, string path) =>
        configuration.GetConfigVar<string>(path);

    /// <summary>
    /// Typed helper to retrieve configuration from environment or appsettings if found.
    /// From the environment the same config key variable will be searched within colon replaced by underline.
    /// </summary>
    /// <param name="path">Variable path (ie. <see cref="CONFIG_KEY_JwtSettings_Key"/></param>    
    public static T GetConfigVar<T>(this IConfiguration configuration, string path)
    {        
        var type = typeof(T);

        if (!type.IsReferenceOrNullableType())
        {
            if (configuration.GetSection(path).Value is null)
                throw new Exception($"Unable to find value for config var [{path}]. Check your user secrets.");
        }

        var value = configuration.GetValue<T>(path);

        return value;
    }

    public static bool IsUnitTest(this IConfiguration configuration) =>
        configuration.GetConfigVar<bool>(CONFIG_KEY_IsUnitTest);

}