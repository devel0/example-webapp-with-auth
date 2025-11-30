namespace ExampleWebApp.Backend.WebApi.Services;

public partial class AliveWebSocketService : WebSocketServiceBase<AliveWSProtocol>
{

    readonly ILogger logger;
    readonly IAuthService auth;

    public AliveWebSocketService(
        IUtilService util,
        ILogger<AliveWebSocketService> logger,
        IAuthService auth
    ) : base(JsonTarget.Basic, util, logger, auth)
    {
        this.logger = logger;
        this.auth = auth;
    }

    protected override async Task OnMessageAsync(
        WebSocketClient<AliveWSProtocol> wsClient, AliveWSProtocol rxObj, string rxOrig, CancellationToken cancellationToken)
    {
        switch (rxObj.MessageType)
        {
            case AliveWSMessageType.Ping:
                {
                    var ping = wsClient.Deserialize<WSPing>(rxOrig);
                    if (ping is not null)
                    {
                        await wsClient.SendAsync(new WSPong(ping.Msg), cancellationToken);
                    }
                }
                break;
        }
    }
}