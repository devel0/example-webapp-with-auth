namespace ExampleWebApp.Backend.WebApi.Services;

public abstract class WebSocketServiceBase<PROTO> : IWebSocketService<PROTO> where PROTO : BaseWSProtocol
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

    static ConcurrentDictionary<WebSocketClient, bool> connections =
        new ConcurrentDictionary<WebSocketClient, bool>();

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
        WebSocketClient wsClient, PROTO rxObj, string rxOrig, CancellationToken cancellationToken);

    public async Task HandleAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var ctx = new WebSocketAcceptContext
        {
            KeepAliveInterval = TimeSpan.FromSeconds(30)
        };
        using var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync(ctx);

        var curUserRes = await auth.CurrentUserNfoAsync(cancellationToken);

        if (curUserRes is null)
        {
            logger.LogError($"can't find valid user on ws conn");
            return;
        }

        var wsCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var wsClient = new WebSocketClient(util, jsonTarget, webSocket);

        connections.TryAdd(wsClient, true);

        logger.LogTrace($"Websocket connected");

        while (!cancellationToken.IsCancellationRequested)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                var rxOrig = await webSocket.ReceiveStringAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(rxOrig))
                {
                    var rxObj = JsonSerializer.Deserialize<BaseWSProtocol>(rxOrig, util.JavaSerializerSettings(wsClient.jsonTarget));

                    if (rxObj is not null)
                    {
                        if (rxObj.BaseProtocolType == BaseWSProtocolType.Custom)
                        {
                            var specificObj = JsonSerializer.Deserialize<PROTO>(rxOrig, util.JavaSerializerSettings(wsClient.jsonTarget));

                            if (specificObj is not null)
                                await OnMessageAsync(wsClient, specificObj, rxOrig, cancellationToken);
                        }

                        else
                        {
                            // handle builtin base messages
                            switch (rxObj.BaseProtocolType)
                            {
                                case BaseWSProtocolType.Ping:
                                    {
                                        var ping = wsClient.Deserialize<BaseWSProtocol>(rxOrig);
                                        if (ping is not null)
                                        {
                                            await wsClient.SendAsync(new BaseWSProtocol
                                            {
                                                BaseProtocolType = BaseWSProtocolType.Pong,
                                                BaseProtocolMsg = ping.BaseProtocolMsg
                                            }, cancellationToken);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
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

        connections.TryRemove(wsClient, out var _);
    }

}
