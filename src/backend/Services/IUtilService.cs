namespace ExampleWebApp.Backend.WebApi.Services;

public enum JsonTarget
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

    JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions options, JsonTarget jsonTarget);

    JsonSerializerOptions JavaSerializerSettings(JsonTarget jsonTarget = JsonTarget.Basic);

    /// <summary>
    /// retrieve <see cref="AppConfig"/> object from configuration
    /// </summary>
    AppConfig GetAppConfig();

}