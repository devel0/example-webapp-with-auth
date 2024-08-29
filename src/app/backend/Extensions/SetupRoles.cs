namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    public static IServiceCollection SetupRoles(this IServiceCollection services)
    {
        var res = services.AddAuthorization(options =>
        {
            options.AddPolicy(ROLE_admin, policy => policy.RequireRole(ROLE_admin));
            options.AddPolicy(ROLE_normal, policy => policy.RequireRole(ROLE_normal));
            options.AddPolicy(ROLE_advanced, policy => policy.RequireRole(ROLE_advanced));
        });

        return res;
    }

}