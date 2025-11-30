namespace ExampleWebApp.Backend.WebApi.Services;

public enum JsonSerializerTarget
{
    /// <summary>
    /// no config applied
    /// </summary>
    None,

    /// <summary>
    /// basic json format ( ignore cycles on ref handler, camel case for property names )
    /// </summary>
    Basic
};

public interface IUtilService
{

    JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions options, JsonSerializerTarget target);

    JsonSerializerOptions JavaSerializerSettings(JsonSerializerTarget target = JsonSerializerTarget.Basic);

    /// <summary>
    /// retrieve <see cref="AppConfig"/> object from configuration
    /// </summary>
    AppConfig GetAppConfig();

}