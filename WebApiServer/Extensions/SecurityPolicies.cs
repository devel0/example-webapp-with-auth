namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Add cors, in development environment, for given corsAllowedOrigins.
    /// </summary>    
    public static void SetupSecurityPolicies(this WebApplicationBuilder webApplicationBuilder,
        string? corsAllowedOrigins)
    {
        if (corsAllowedOrigins is not null)
        {
            webApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy(APP_CORS_POLICY_NAME, policy =>
                {
                    policy
                        .WithOrigins(corsAllowedOrigins.Split(','))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    } 

}