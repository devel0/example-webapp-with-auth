namespace ExampleWebApp.Backend.WebApi;

public partial class UtilService :
    IUtilService
{

    readonly ILogger logger;
    readonly IServiceProvider serviceProvider;

    public UtilService(
        IServiceProvider serviceProvider,
        ILogger<UtilService> logger
        )
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions options)
    {
        options.Converters.Add(new JsonStringEnumConverter());
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles; // TOOD: verify if can use All        

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

}