using System.Text.RegularExpressions;

namespace ExampleWebApp.Backend.Abstractions;

public class ReflectionHelper<T>
{
    public static string GetPath<TProperty>(Expression<Func<T, TProperty>> expr)
    {
        var name = expr.Parameters[0].Name;

        return expr.Body.ToString().Replace($"{name} => {name}", typeof(T).Name);
    }
}

public static class Extensions
{

    /// <summary>
    /// retrieve <see cref="Types.AppConfig"/> from configuration
    /// </summary>
    public static AppConfig AppConfig(this IConfiguration configuration)
    {
        var appConfig = configuration.GetSection(CONFIG_KEY_AppConfig).Get<AppConfig>();

        if (appConfig is null)
            throw new Exception($"Can't load {nameof(Types.AppConfig)} object from appsettings.json");

        return appConfig;
    }

    /// <summary>
    /// change a value in <see cref="Types.AppConfig"/> reflecting to the configuration section
    /// </summary>
    public static void SetValue<P, T>(this AppConfig appConfig,
        IConfiguration configuration, Expression<Func<AppConfig, P>> path, T value) =>
        Types.AppConfig.SetValue(configuration, path, value);

}
