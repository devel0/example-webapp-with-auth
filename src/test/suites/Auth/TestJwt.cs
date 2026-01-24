namespace ExampleWebApp.Backend.Test.Auth;

public class TestJwt
  : IClassFixture<WebApplicationFactory<Program>>
{
    readonly ITestOutputHelper testOutput;

    public TestJwt(
        ITestOutputHelper testOutputHelper
        )
    {
        testOutput = testOutputHelper;
    }

    /// <summary>
    /// Test access token invalid with fake token.
    /// </summary>
    [Fact]
    public async Task AccessTokenInvalidWithFake()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(300);
        var refreshTokenDuration = TimeSpan.FromSeconds(0); // disabled

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var jwtService = testFactory.Services.GetRequiredService<IJWTService>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        var appConfig = config.GetAppConfig();
        appConfig.SetValue(config, x => x.Auth.Jwt.ClockSkew, TimeSpan.Zero);
        appConfig.SetValue(config, x => x.Auth.Jwt.AccessTokenDuration, accessTokenDuration);
        appConfig.SetValue(config, x => x.Auth.Jwt.RefreshTokenDuration, refreshTokenDuration);        

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var jwtCookies = loginRes.Headers.GetJwtCookiesFromResponse();
        Assert.NotNull(jwtCookies.AccessToken);

        // set a fake access token

        var fakeAccessToken = GenerateFakeAccessToken(config, jwtService, jwtCookies.AccessToken);

        client.SetCookie(WEB_CookieName_XAccessToken, fakeAccessToken);

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);
    }

    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task AccessTokenExpiredValidRefreshToken()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);
        var refreshTokenDuration = TimeSpan.FromSeconds(300);

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        var appConfig = config.GetAppConfig();
        appConfig.SetValue(config, x => x.Auth.Jwt.ClockSkew, TimeSpan.Zero);
        appConfig.SetValue(config, x => x.Auth.Jwt.AccessTokenDuration, accessTokenDuration);
        appConfig.SetValue(config, x => x.Auth.Jwt.RefreshTokenDuration, refreshTokenDuration);

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        await Task.Delay(accessTokenDuration);

        // access token now expired but refresh token allow to resume

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);
    }

    /// <summary>
    /// States that access token not valid after expiration if refresh token already expired
    /// </summary>
    [Fact]
    public async Task AccessTokenExpiredRefreshTokenExpired()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);
        var refreshTokenDuration = TimeSpan.FromSeconds(2);

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        var appConfig = config.GetAppConfig();
        appConfig.SetValue(config, x => x.Auth.Jwt.ClockSkew, TimeSpan.Zero);
        appConfig.SetValue(config, x => x.Auth.Jwt.AccessTokenDuration, accessTokenDuration);
        appConfig.SetValue(config, x => x.Auth.Jwt.RefreshTokenDuration, refreshTokenDuration);

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        await Task.Delay(refreshTokenDuration);

        // access token and refresh token now expired

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);
    }

    /// <summary>
    /// Renewal of refresh token allow to slide the expiration.
    /// </summary>
    [Fact]
    public async Task RenewRefreshToken()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);
        var refreshTokenDuration = TimeSpan.FromSeconds(3);
        var refreshTokenRotationSkew = accessTokenDuration + TimeSpan.FromSeconds(1);

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var logger = testFactory.Services.GetRequiredService<ILogger<TestBasicAuth>>();
        var dbContext = testFactory.Services.GetRequiredService<AppDbContext>();
        var util = testFactory.Services.GetRequiredService<IUtilService>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        var appConfig = config.GetAppConfig();

        appConfig.SetValue(config, x => x.Auth.Jwt.ClockSkew, TimeSpan.Zero);
        appConfig.SetValue(config, x => x.Auth.Jwt.AccessTokenDuration, accessTokenDuration);
        appConfig.SetValue(config, x => x.Auth.Jwt.RefreshTokenDuration, refreshTokenDuration);

        logger.LogTrace("Login");

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var loginResObj = await loginRes.DeserializeAsync<LoginResponseDto>(util);
        Assert.NotNull(loginResObj);

        var toWait = accessTokenDuration / 3;

        await Task.Delay(toWait);

        // access token still valid

        var renewRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.RenewAccessToken)}")).ApplySetCookies(client);

        toWait = refreshTokenDuration - TimeSpan.FromSeconds(1);

        await Task.Delay(toWait);

        // refresh token still valid because renewed

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

        // last renew
        logger.LogTrace($"3) Last renew refresh token");
        renewRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.RenewRefreshToken)}")).ApplySetCookies(client);

        // leave refresh expire using the refresh token provided configuration duration

        toWait = refreshTokenDuration + TimeSpan.FromSeconds(1);
        logger.LogTrace($"4) Wait ( {toWait.TotalSeconds} sec ) refresh token to expire");

        await Task.Delay(toWait);

        currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);
    }

    /// <summary>
    /// Invalid renew refresh token on disabled user.
    /// </summary>
    [Fact]
    public async Task InvalidRenewRefreshTokenOnDisabledUser()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);
        var refreshTokenDuration = TimeSpan.FromSeconds(3);
        var refreshTokenRotationSkew = accessTokenDuration + TimeSpan.FromSeconds(1);

        using var adminTestFactory = new TestFactory();
        await adminTestFactory.InitAsync();
        var adminClient = adminTestFactory.Client;
        var adminConfig = adminTestFactory.Services.GetRequiredService<IConfiguration>();
        var logger = adminTestFactory.Services.GetRequiredService<ILogger<TestBasicAuth>>();
        var util = adminTestFactory.Services.GetRequiredService<IUtilService>();

        var adminUserConfig = adminConfig.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        //

        using var userTestFactory = new TestFactory();
        await userTestFactory.InitAsync(dropDb: false);
        var userClient = userTestFactory.Client;
        var userConfig = userTestFactory.Services.GetRequiredService<IConfiguration>();

        var appConfig = userConfig.GetAppConfig();
        appConfig.SetValue(userConfig, x => x.Auth.Jwt.ClockSkew, TimeSpan.Zero);
        appConfig.SetValue(userConfig, x => x.Auth.Jwt.AccessTokenDuration, accessTokenDuration);
        appConfig.SetValue(userConfig, x => x.Auth.Jwt.RefreshTokenDuration, refreshTokenDuration);

        var loginRes = (await adminClient.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(adminClient);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        // create normal user

        var normalUsername = "normalUsername";
        var normalEmail = "normal@test.com";
        var normalPassword = "normalPass1!";
        var normalRoles = new[] { ROLE_normal };

        var editUserRes = (await adminClient.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            EditUsername = normalUsername,
            EditEmail = normalEmail,
            EditPassword = normalPassword,
            EditRoles = normalRoles
        })).ApplySetCookies(adminClient);

        Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

        // login normal user

        loginRes = (await userClient.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = normalUsername,
            Password = normalPassword
        })).ApplySetCookies(userClient);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var loginResObj = await loginRes.DeserializeAsync<LoginResponseDto>(util);
        Assert.NotNull(loginResObj);

        var toWait = loginResObj.RefreshTokenExpiration - DateTimeOffset.UtcNow - TimeSpan.FromSeconds(1);
        logger.LogTrace($"1) Wait ( {toWait.TotalSeconds} sec ) refresh token about to expire");

        await Task.Delay(toWait);

        // refresh token still valid, now slide a new refresh token through renew refresh token

        var renewRes = (await userClient.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.RenewRefreshToken)}")).ApplySetCookies(userClient);
        var currentUserRes = (await userClient.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(userClient);

        Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

        // disable user
        logger.LogTrace($"2) admin disable the user");

        editUserRes = (await adminClient.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            ExistingUsername = normalUsername,
            EditDisabled = true
        })).ApplySetCookies(adminClient);

        toWait = refreshTokenDuration - TimeSpan.FromSeconds(1);
        logger.LogTrace($"2) Wait ( {toWait.TotalSeconds} sec ) refresh token about to expire");

        await Task.Delay(toWait);

        // refresh token not valid because user disabled meanwhile

        currentUserRes = (await userClient.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(userClient);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);
    }

    /// <summary>
    /// Refresh token removed from db after logout.
    /// </summary>
    [Fact]
    public async Task RefreshTokenAfterLogout()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var logger = testFactory.Services.GetRequiredService<ILogger<TestBasicAuth>>();
        var dbContext = testFactory.Services.GetRequiredService<AppDbContext>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        logger.LogTrace("Login");
        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var refreshTokensCount = dbContext.UserRefreshTokens.Count(w => w.UserName == "admin");
        logger.LogTrace($"refresh tokens in db {refreshTokensCount}");
        Assert.Equal(1, refreshTokensCount);

        logger.LogTrace("Logout");
        var logoutRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.Logout)}")).ApplySetCookies(client);
        Assert.Equal(HttpStatusCode.OK, logoutRes.StatusCode);

        refreshTokensCount = dbContext.UserRefreshTokens.Count(w => w.UserName == "admin");
        logger.LogTrace($"refresh tokens in db {refreshTokensCount}");
        Assert.Equal(0, refreshTokensCount);
    }

    /// <summary>
    /// Invalid refresh token on disabled user.
    /// </summary>
    [Fact]
    public async Task InvalidRefreshTokenOnDisabledUser()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);

        using var adminTestFactory = new TestFactory();
        await adminTestFactory.InitAsync();
        var adminClient = adminTestFactory.Client;
        var adminConfig = adminTestFactory.Services.GetRequiredService<IConfiguration>();
        var logger = adminTestFactory.Services.GetRequiredService<ILogger<TestBasicAuth>>();

        var adminUserConfig = adminConfig.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        var appConfig = adminConfig.GetAppConfig();
        appConfig.SetValue(adminConfig, x => x.Auth.Jwt.ClockSkew, TimeSpan.Zero);
        appConfig.SetValue(adminConfig, x => x.Auth.Jwt.AccessTokenDuration, accessTokenDuration);

        //

        using var userTestFactory = new TestFactory();
        await userTestFactory.InitAsync(dropDb: false);
        var userClient = userTestFactory.Client;
        var userConfig = userTestFactory.Services.GetRequiredService<IConfiguration>();

        var userAppConfig = userConfig.GetAppConfig();
        appConfig.SetValue(userConfig, x => x.Auth.Jwt.ClockSkew, TimeSpan.Zero);
        appConfig.SetValue(userConfig, x => x.Auth.Jwt.AccessTokenDuration, accessTokenDuration);

        //

        var loginRes = (await adminClient.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(adminClient);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        // create normal user

        var normalUsername = "normalUsername";
        var normalEmail = "normal@test.com";
        var normalPassword = "normalPass1!";
        var normalRoles = new[] { ROLE_normal };

        var editUserRes = (await adminClient.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            EditUsername = normalUsername,
            EditEmail = normalEmail,
            EditPassword = normalPassword,
            EditRoles = normalRoles
        })).ApplySetCookies(adminClient);

        Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

        // login normal user

        loginRes = (await userClient.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = normalUsername,
            Password = normalPassword
        })).ApplySetCookies(userClient);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        logger.LogTrace($"waiting user access token expire");
        await Task.Delay(accessTokenDuration);

        // disable user

        editUserRes = (await adminClient.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            ExistingUsername = normalUsername,
            EditDisabled = true
        })).ApplySetCookies(adminClient);

        // test current user can't use refresh token

        var currentUserRes = (await userClient.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}"))
            .ApplySetCookies(userClient);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);
    }

    /// <summary>
    /// Test access token valid from, to params.
    /// </summary>
    [Fact]
    public async Task AccessTokenValidFromTo()
    {
        var DRIFT = TimeSpan.FromSeconds(1);

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var jwtService = testFactory.Services.GetRequiredService<IJWTService>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

        var appConfig = config.GetAppConfig();
        var accessTokenDuration = appConfig.Auth.Jwt.AccessTokenDuration;

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        var dtStart = DateTimeOffset.UtcNow;

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var jwtCookies = loginRes.Headers.GetJwtCookiesFromResponse();
        Assert.NotNull(jwtCookies.AccessToken);
        var jwt = jwtService.DecodeAccessToken(jwtCookies.AccessToken);
        Assert.NotNull(jwt);

        // jwt issued >= req start
        Assert.True(jwt.IssuedAt + DRIFT >= dtStart);

        // jet valid from >= req start
        Assert.True(jwt.ValidFrom + DRIFT >= dtStart);

        // jwt from + duration = to
        Assert.Equal(jwt.ValidFrom + accessTokenDuration, jwt.ValidTo);
    }

}