namespace ExampleWebApp.Backend.WebApi.Services;

/// <summary>
/// Handle websocket connections
/// </summary>
public interface IWebSocketService<PROTO> where PROTO : class
{

    /// <summary>
    /// Manage websocket connection and spawn a ws handler.
    /// </summary>
    Task HandleAsync(HttpContext httpContext, CancellationToken cancellationToken);

    /// <summary>
    /// send obj to all connected ws clients
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="skipDuplicates">do not send the obj if another equals object was sent to the same client</param>    
    Task SendToAllClientsAsync(PROTO obj, bool skipDuplicates, CancellationToken cancellationToken);

}
