namespace ExampleWebApp.Backend.WebApi.Services;

public class WebSocketClient
{
    readonly IUtilService util;

    public JsonTarget jsonTarget { get; }

    readonly WebSocket webSocket;

    SemaphoreSlim SendSem { get; } = new SemaphoreSlim(1, 1);

    public string? LastSendSerialized { get; private set; }

    public WebSocketClient(IUtilService util, JsonTarget jsonTarget, WebSocket webSocket)
    {
        this.util = util;
        this.jsonTarget = jsonTarget;
        this.webSocket = webSocket;
    }

    public async Task SendAsync(object obj, CancellationToken cancellationToken, bool skipDuplicates = false)
    {
        await SendSem.WaitAsync(cancellationToken);

        try
        {
            var str = JsonSerializer.Serialize(obj, util.JavaSerializerSettings(jsonTarget));

            if (!skipDuplicates || str != LastSendSerialized)
            {
                await webSocket.SendStringAsync(str, cancellationToken);

                LastSendSerialized = str;
            }
        }
        finally
        {
            SendSem.Release();
        }
    }

    public SPECIFIC? Deserialize<SPECIFIC>(string originalSocketMessage) where SPECIFIC : BaseWSProtocol
    {
        var specific = JsonSerializer.Deserialize<SPECIFIC>(
            originalSocketMessage ?? "{}", util.JavaSerializerSettings(jsonTarget));

        return specific;
    }

}
