namespace ExampleWebApp.Backend.Data;

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
    /// <param name="path">Variable path (ie. <see cref="CONFIG_KEY_DbSchemaSnakeCase"/></param>    
    /// <param name="configuration">asp net configuration</param>    
    public static string GetConfigVar(this IConfiguration configuration, string path) =>
        configuration.GetConfigVar<string>(path);

    static bool IsReferenceOrNullableType(Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

    /// <summary>
    /// Typed helper to retrieve configuration from environment or appsettings if found.
    /// From the environment the same config key variable will be searched within colon replaced by underline.
    /// </summary>
    /// <param name="path">Variable path (ie. <see cref="CONFIG_KEY_DbSchemaSnakeCase"/></param>    
    /// <param name="configuration">asp net configuration</param>    
    public static T GetConfigVar<T>(this IConfiguration configuration, string path)
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

}