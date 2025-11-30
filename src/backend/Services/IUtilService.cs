namespace ExampleWebApp.Backend.WebApi.Services;

public interface IUtilService
{

    JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions options);

    JsonSerializerOptions JavaSerializerSettings { get; }

    /// <summary>
    /// retrieve <see cref="AppConfig"/> object from configuration
    /// </summary>
    AppConfig GetAppConfig();

    /// <summary>
    /// Receive message from websocket using default java serializer and service logger;
    /// returns string original response to allow further specialized deserialization.
    /// </summary>            
    Task<WSObjNfo<PROTO>> ReceiveMessageAsync<PROTO>(WebSocket webSocket, CancellationToken cancellationToken);

    /// <summary>
    /// Send string to given websocket
    /// </summary>
    Task<bool> SendMessageSerializedAsync(string str, WebSocket webSocket, CancellationToken cancellationToken);

    /// <summary>
    /// Send object to given websocket through serialization.
    /// </summary>
    Task<bool> SendMessageAsync(object msg, WebSocket webSocket, CancellationToken cancellationToken);

}