namespace ExampleWebApp.Backend.WebApi.Services;

public abstract class WebSocketServiceBase<PROTO> : IWebSocketService<PROTO>
{
    readonly ILogger logger;
    readonly IUtilService util;
    readonly IAuthService auth;

    public WebSocketServiceBase(
      IUtilService util,
      ILogger logger,
      IAuthService auth
  )
    {
        this.util = util;
        this.logger = logger;
        this.auth = auth;
    }

    static ConcurrentDictionary<WebSocketNfo, bool> connections = new ConcurrentDictionary<WebSocketNfo, bool>();

    async Task Send(WebSocketNfo wsNfo, object obj, CancellationToken cancellationToken, bool skipDuplicates)
    {
        await wsNfo.SendSem.WaitAsync(cancellationToken);

        try
        {
            var str = JsonSerializer.Serialize(obj, util.JavaSerializerSettings);

            if (!skipDuplicates || str != wsNfo.SendTimestamp)
            {
                await util.SendMessageSerializedAsync(str, wsNfo.webSocket, cancellationToken);

                wsNfo.SendTimestamp = str;
            }
        }
        finally
        {
            wsNfo.SendSem.Release();
        }
    }

    public async Task SendToAllClientsAsync(PROTO obj, CancellationToken cancellationToken, bool skipDuplicates)
    {    
        var clients = connections.ToList().Select(w => w.Key).ToList();

        foreach (var client in clients)
        {
            await Send(client, obj, cancellationToken, skipDuplicates);
        }
    }

    protected abstract Task OnMessageAsync(WebSocket webSocket, WSObjNfo<PROTO> mex, CancellationToken cancellationToken);

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

        var wsNfo = new WebSocketNfo(webSocket);

        connections.TryAdd(wsNfo, true);

        logger.LogTrace($"Websocket connected");

        while (!cancellationToken.IsCancellationRequested)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                var rxObjNfo = await util.ReceiveMessageAsync<PROTO>(webSocket, cancellationToken);

                if (rxObjNfo.Obj is not null && rxObjNfo.Str is not null)
                    await OnMessageAsync(webSocket, rxObjNfo, cancellationToken);
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
