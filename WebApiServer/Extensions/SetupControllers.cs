namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    public static IMvcBuilder SetupControllers(this IServiceCollection serviceCollection)
    {
        var res = serviceCollection.AddControllers(options =>
        {
            options.Filters.Add<JWTFilter>();
        });

        return res;
    }

}