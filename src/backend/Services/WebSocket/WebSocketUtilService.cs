namespace ExampleWebApp.Backend.WebApi.Services;

public partial class WebSocketUtilService : IWebSocketUtilService
{

    readonly ILogger logger;
    readonly IServiceProvider serviceProvider;
    readonly IConfiguration configuration;
    readonly IUtilService util;

    public WebSocketUtilService(
        IServiceProvider serviceProvider,
        IUtilService util,
        ILogger<WebSocketUtilService> logger,
        IConfiguration configuration
        )
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.configuration = configuration;
        this.util = util;
    }

    public async Task<bool> SendMessageSerializedAsync(
        string str, WebSocket webSocket, CancellationToken cancellationToken)
    {
        var res = await webSocket.SendStringAsync(str, cancellationToken);

        return res;
    }

    public async Task<WSObjNfo<PROTO>> ReceiveMessageAsync<PROTO>(
        WebSocket webSocket, JsonSerializerTarget jsonTarget, CancellationToken cancellationToken)
    {
        PROTO? res = default;
        var str = await webSocket.ReceiveStringAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(str))
            res = JsonSerializer.Deserialize<PROTO>(str, util.JavaSerializerSettings(jsonTarget));

        return new WSObjNfo<PROTO> { Obj = res, Str = str };
    }    

    public async Task<bool> SendMessageAsync(
        object msg, WebSocket webSocket, JsonSerializerTarget jsonTarget, CancellationToken cancellationToken)
    {
        var str = JsonSerializer.Serialize(msg, util.JavaSerializerSettings(jsonTarget));

        var res = await webSocket.SendStringAsync(str, cancellationToken);

        return res;
    }

}