namespace ExampleWebApp.Backend.WebApi.Services;

public partial class AliveWebSocketService : WebSocketServiceBase<AliveWSProtocol>
{

    readonly ILogger logger;
    readonly IUtilService util;
    readonly IAuthService auth;

    public AliveWebSocketService(
        IUtilService util,
        IWebSocketUtilService wsUtil,
        ILogger<AliveWebSocketService> logger,
        IAuthService auth
    ) : base(JsonSerializerTarget.Basic, util, wsUtil, logger, auth)
    {
        this.util = util;
        this.logger = logger;
        this.auth = auth;
    }

    protected override async Task OnMessageAsync(
        WebSocket webSocket, WSObjNfo<AliveWSProtocol> mex, CancellationToken cancellationToken)
    {
        if (mex.Obj is null) return;

        switch (mex.Obj.MessageType)
        {
            case AliveWSMessageType.Ping:
                {
                    var ping = JsonSerializer.Deserialize<WSPing>(mex.Str ?? "{}", util.JavaSerializerSettings(jsonTarget));
                    if (ping is not null)
                    {
                        await wsUtil.SendMessageAsync(new WSPong(ping.Msg), webSocket, jsonTarget, cancellationToken);
                    }
                }
                break;
        }
    }
}