namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Seed database admin user and roles.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication webApplication,
        CancellationToken cancellationToken)
    {
        var logger = webApplication.Logger;

        using var scope = webApplication.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usermgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var rolemgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var config = webApplication.Configuration;

        var appConfig = config.AppConfig();

        foreach (var seedUser in appConfig.Database.Seed.Users)
        {            
            var q = await usermgr.FindByNameAsync(seedUser.Username);

            if (q is not null) continue;

            var user = new ApplicationUser
            {
                UserName = seedUser.Username,
                Email = seedUser.Email
            };

            var res = await usermgr.CreateAsync(user, seedUser.Password);
            if (!res.Succeeded)
                throw new Exception($"Unable to create initial user {string.Join(";", res.Errors.Select(w => w.Description))}");

            foreach (var role in seedUser.Roles)
                await rolemgr.CreateAsync(new IdentityRole(role));

            await usermgr.AddToRoleAsync(user, ROLE_admin);
            var confirmEmailToken = await usermgr.GenerateEmailConfirmationTokenAsync(user);
            await usermgr.ConfirmEmailAsync(user, confirmEmailToken);

            // logger.LogInformation($"Created admin email:{adminEmail} pass:{adminPassword}");
        }

    }

}