namespace ExampleWebApp.Backend.Test.Auth;

public class TestBasicAuth
  : IClassFixture<WebApplicationFactory<Program>>
{
    readonly ITestOutputHelper testOutput;

    public TestBasicAuth(
        ITestOutputHelper testOutputHelper
        )
    {
        testOutput = testOutputHelper;
    }

     /// <summary>
    /// Use ListUsers to test the default admin user seed executed.
    /// </summary>
    [Fact]
    public async Task SeedDefaultAdmin()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var util = testFactory.Services.GetRequiredService<IUtilService>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminEmail = adminUserConfig.Email;
        var adminPassword = adminUserConfig.Password;

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var listUsersRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.ListUsers)}/")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, listUsersRes.StatusCode);

        var listUsers = await listUsersRes.DeserializeAsync<List<UserListItemResponseDto>>(util);

        Assert.NotNull(listUsers);
        Assert.Single(listUsers);
        Assert.Equal(adminUsername, listUsers[0].UserName);
        Assert.Equal(adminEmail, listUsers[0].Email);
        Assert.Equal(new List<string> { ROLE_admin }, listUsers[0].Roles);
    }

    /// <summary>
    /// Test login admin works with username.
    /// </summary>
    [Fact]
    public async Task ValidAdminLogin()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        // login with username

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);
    }

    /// <summary>
    /// Test login admin works with email.
    /// </summary>
    [Fact]
    public async Task ValidAdminLoginWithEmail()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminEmail = adminUserConfig.Email;
        var adminPassword = adminUserConfig.Password;

        // login with email

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminEmail,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);
    }

    /// <summary>
    /// Test admin current user works after valid username login
    /// </summary>
    [Fact]
    public async Task ValidAdminCurrentUser()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var util = testFactory.Services.GetRequiredService<IUtilService>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminEmail = adminUserConfig.Email;
        var adminPassword = adminUserConfig.Password;
        var adminRoles = adminUserConfig.Roles;

        // login with username

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        // retrieve current user

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

        var currentUser = await currentUserRes.DeserializeAsync<CurrentUserResponseDto>(util);

        Assert.NotNull(currentUser);
        Assert.Equal(adminUsername, currentUser.UserName);
        Assert.Equal(adminEmail, currentUser.Email);
        Assert.Equal(adminRoles, currentUser.Roles);
    }

    /// <summary>
    /// Test admin login with username and invalid password not succeeded.
    /// </summary>
    [Fact]
    public async Task InvalidAdminLogin()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password + "WRONG";

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, loginRes.StatusCode);
    }

    /// <summary>
    /// Test admin login with email and invalid password not succeeded.
    /// </summary>
    [Fact]
    public async Task InvalidAdminLoginWithEmail()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminEmail = adminUserConfig.Email;
        var adminPassword = adminUserConfig.Password + "WRONG";

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminEmail,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, loginRes.StatusCode);
    }

    /// <summary>
    /// Test logout.
    /// </summary>
    [Fact]
    public async Task UnauthorizedAfterLogout()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var logoutRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.Logout)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, logoutRes.StatusCode);

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);
    }

}