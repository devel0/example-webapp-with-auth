namespace ExampleWebApp.Backend.WebApi.Services;

public partial class ExampleWebSocketService : WebSocketServiceBase<ExampleWSProtocol>
{

    readonly ILogger logger;
    readonly IAuthService auth;

    public ExampleWebSocketService(
        IUtilService util,
        ILogger<ExampleWebSocketService> logger,
        IAuthService auth
    ) : base(JsonTarget.Basic, util, logger, auth)
    {
        this.logger = logger;
        this.auth = auth;
    }

    protected override async Task OnMessageAsync(
        WebSocketClient wsClient, ExampleWSProtocol rxObj, string rxOrig, CancellationToken cancellationToken)
    {
        switch (rxObj.ProtocolType)
        {
            case ExampleWSProtocolType.MyProto1:
                {
                    var customMex = wsClient.Deserialize<ExampleWSProto1>(rxOrig);
                    if (customMex is not null)
                    {
                        logger.LogDebug($"rx [{customMex.SomeMsg}] mex from client");
                    }
                }
                break;

            case ExampleWSProtocolType.MyProto2:
                {
                    var customMex = wsClient.Deserialize<ExampleWSProto2>(rxOrig);
                    if (customMex is not null)
                    {
                        logger.LogDebug($"rx [{customMex.SomeLongValue}] value from client");
                    }
                }
                break;
        }
    }
}