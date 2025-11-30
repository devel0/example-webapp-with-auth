namespace ExampleWebApp.Backend.WebApi.Services;

public abstract class WebSocketServiceBase<PROTO> : IWebSocketService<PROTO> where PROTO : class
{
    protected readonly ILogger logger;
    protected readonly IAuthService auth;
    protected readonly IUtilService util;
    protected readonly JsonTarget jsonTarget;

    public WebSocketServiceBase(
        JsonTarget jsonTarget,
        IUtilService util,
        ILogger logger,
        IAuthService auth
    )
    {
        this.jsonTarget = jsonTarget;
        this.util = util;
        this.logger = logger;
        this.auth = auth;
    }

    static ConcurrentDictionary<WebSocketClient<PROTO>, bool> connections =
        new ConcurrentDictionary<WebSocketClient<PROTO>, bool>();

    public async Task SendToAllClientsAsync(PROTO obj, bool skipDuplicates, CancellationToken cancellationToken)
    {
        var clients = connections.ToList().Select(w => w.Key).ToList();

        foreach (var client in clients)
        {
            if (obj is not null)
                await client.SendAsync(obj, cancellationToken, skipDuplicates);
        }
    }

    /// <summary>
    /// switch on <paramref name="rxObj"/> message type to deserialize <paramref name="rxOrig"/> further
    /// on specific expected object and handle
    /// </summary>
    protected abstract Task OnMessageAsync(
        WebSocketClient<PROTO> wsClient, PROTO rxObj, string rxOrig, CancellationToken cancellationToken);

    public async Task HandleAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var ctx = new WebSocketAcceptContext
        {
            KeepAliveInterval = TimeSpan.FromSeconds(30)
        };
        using var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync(ctx);

        var curUserRes = auth.CurrentUserNfo();

        if (curUserRes is null)
        {
            logger.LogError($"can't find valid user on ws conn");
            return;
        }

        var wsCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var wsNfo = new WebSocketClient<PROTO>(util, jsonTarget, webSocket);

        connections.TryAdd(wsNfo, true);

        logger.LogTrace($"Websocket connected");

        while (!cancellationToken.IsCancellationRequested)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                var str = await webSocket.ReceiveStringAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(str))
                {
                    var rxObj = JsonSerializer.Deserialize<PROTO>(str, util.JavaSerializerSettings(wsNfo.jsonTarget));

                    if (rxObj is not null)
                        await OnMessageAsync(wsNfo, rxObj, str, cancellationToken);
                }
            }
            else break;
        }

        try
        {
            wsCts.Cancel();

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                statusDescription: "", cancellationToken: cancellationToken);
        }
        catch (WebSocketException wex)
        {
            logger.LogWarning(wex, "websocket exception");
        }

        logger.LogTrace($"Web socket closed");

        connections.TryRemove(wsNfo, out var _);
    }

}
