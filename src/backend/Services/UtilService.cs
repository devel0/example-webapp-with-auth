namespace ExampleWebApp.Backend.WebApi.Services;

public partial class UtilService :
    IUtilService
{

    readonly ILogger logger;
    readonly IServiceProvider serviceProvider;
    readonly IConfiguration configuration;

    public UtilService(
        IServiceProvider serviceProvider,
        ILogger<UtilService> logger,
        IConfiguration configuration
        )
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.configuration = configuration;
    }

    public AppConfig GetAppConfig() => configuration.GetAppConfig();

    public JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions options)
    {
        options.Converters.Add(new JsonStringEnumConverter());

        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // ignore by attribute example

        // options.TypeInfoResolver = new DefaultJsonTypeInfoResolver
        // {
        //     Modifiers = {
        //         (t) =>
        //         {
        //             foreach (var prop in t.Properties)
        //             {
        //                 var toignore = prop.AttributeProvider?
        //                     .GetCustomAttributes(false)
        //                     .OfType<SomeAttribute>()
        //                     .Count() > 0;

        //                 if (toignore)
        //                 {
        //                     prop.ShouldSerialize = (obj, _) => { return false; };
        //                 }
        //             }
        //         }
        //     }
        // };

        return options;
    }

    public JsonSerializerOptions JavaSerializerSettings
    {
        get
        {
            var options = new JsonSerializerOptions();

            ConfigureJsonSerializerOptions(options);

            return options;
        }
    }

    public async Task<WSObjNfo<PROTO>> ReceiveMessageAsync<PROTO>(WebSocket webSocket, CancellationToken cancellationToken)
    {
        PROTO? res = default;
        var str = await webSocket.ReceiveStringAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(str))
            res = JsonSerializer.Deserialize<PROTO>(str, JavaSerializerSettings);

        return new WSObjNfo<PROTO> { Obj = res, Str = str };
    }

    public async Task<bool> SendMessageSerializedAsync(string str, WebSocket webSocket, CancellationToken cancellationToken)
    {
        var res = await webSocket.SendStringAsync(str, cancellationToken);

        return res;
    }

    public async Task<bool> SendMessageAsync(object msg, WebSocket webSocket, CancellationToken cancellationToken)
    {
        var str = JsonSerializer.Serialize(msg, JavaSerializerSettings);

        var res = await webSocket.SendStringAsync(str, cancellationToken);

        return res;
    }

}