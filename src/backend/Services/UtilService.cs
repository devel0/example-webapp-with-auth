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

    public JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions options, JsonTarget target)
    {
        if (target == JsonTarget.None) return options;

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

    public JsonSerializerOptions JavaSerializerSettings(JsonTarget target)
    {
        var options = new JsonSerializerOptions();

        ConfigureJsonSerializerOptions(options, target);

        return options;
    }

}