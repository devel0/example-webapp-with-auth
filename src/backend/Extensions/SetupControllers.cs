namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    public static IMvcBuilder SetupControllers(this WebApplicationBuilder builder)
    {
        var sp = builder.Services.BuildServiceProvider();

        var res = builder.Services.AddControllers(options =>
        {
            // options.Filters.Add<JWTFilter>();
        })

        .AddJsonOptions(options =>
        {
            var util = sp.GetRequiredService<IUtilService>();

            util.ConfigureJsonSerializerOptions(options.JsonSerializerOptions);
        });

        return res;        
    }

}