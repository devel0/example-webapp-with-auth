namespace ExampleWebApp.Backend.WebApi.Services.Abstractions;

public interface IUtilService
{

    JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions options);

    JsonSerializerOptions JavaSerializerSettings { get; }

    /// <summary>
    /// retrieve <see cref="Types.AppConfig"/> object from configuration
    /// </summary>
    AppConfig GetAppConfig();

}