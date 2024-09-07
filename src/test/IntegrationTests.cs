namespace Test;

public class IntegrationTests
  : IClassFixture<WebApplicationFactory<Program>>
{
    readonly ITestOutputHelper testOutput;

    public IntegrationTests(
        ITestOutputHelper testOutputHelper
        )
    {
        this.testOutput = testOutputHelper;
    }

    static readonly string AuthApiPrefix = $"{API_PREFIX}/{nameof(AuthController).StripEnd("Controller")}";

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

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

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

        var adminEmail = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Email);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

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

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminEmail = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Email);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);
        var adminRoles = new List<string> { ROLE_admin };

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

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password) + "WRONG";

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

        var adminEmail = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Email);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password) + "WRONG";

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminEmail,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, loginRes.StatusCode);
    }

    /// <summary>
    /// States that access token not valid after expiration if refresh token already expired
    /// </summary>
    [Fact]
    public async Task AccessTokenExpiration()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);
        var refreshTokenDuration = TimeSpan.FromSeconds(0); // disabled

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        config.SetConfigVar(CONFIG_KEY_JwtSettings_ClockSkewSeconds, "0");
        config.SetConfigVar(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds, accessTokenDuration.TotalSeconds.ToString());
        config.SetConfigVar(CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds, refreshTokenDuration.TotalSeconds.ToString());

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        await Task.Delay(accessTokenDuration);

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);
    }

    /// <summary>
    /// Renew access token when refresh token still valid.
    /// </summary>
    [Fact]
    public async Task RenewAccessToken()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);
        var refreshTokenDuration = TimeSpan.FromSeconds(3);

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        config.SetConfigVar(CONFIG_KEY_JwtSettings_ClockSkewSeconds, "0");
        config.SetConfigVar(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds, accessTokenDuration.TotalSeconds.ToString());
        config.SetConfigVar(CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds, refreshTokenDuration.TotalSeconds.ToString());

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        await Task.Delay(accessTokenDuration);

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);
    }

    /// <summary>
    /// Test refresh token can't be used twice because rotated.
    /// </summary>
    [Fact]
    public async Task RotateRefreshToken()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);
        var refreshTokenDuration = TimeSpan.FromMinutes(10);
        var refreshTokenRotationSkew = accessTokenDuration + TimeSpan.FromSeconds(1);

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var logger = testFactory.Services.GetRequiredService<ILogger<IntegrationTests>>();
        var dbContext = testFactory.Services.GetRequiredService<AppDbContext>();

        config.SetConfigVar(CONFIG_KEY_JwtSettings_ClockSkewSeconds, "0");

        config.SetConfigVar(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds,
            accessTokenDuration.TotalSeconds.ToString());

        config.SetConfigVar(CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds,
            refreshTokenDuration.TotalSeconds.ToString());

        config.SetConfigVar(CONFIG_KEY_JwtSettings_RefreshTokenRotationSkewSeconds,
            refreshTokenRotationSkew.TotalSeconds.ToString());

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

        logger.LogTrace("Login");

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var jwtCookies = loginRes.Headers.GetJwtCookiesFromResponse();
        Assert.NotNull(jwtCookies.AccessToken);

        var refreshToken1 = HttpUtility.UrlDecode(jwtCookies.RefreshToken);
        Assert.NotNull(refreshToken1);

        logger.LogTrace($"1) Wait access token expires ( refreshToken: {refreshToken1} )");

        await Task.Delay(accessTokenDuration);

        // access token now expired, then refresh token will be used and rotated in next call

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

        jwtCookies = currentUserRes.Headers.GetJwtCookiesFromResponse();
        Assert.NotNull(jwtCookies.AccessToken);

        var refreshToken2 = HttpUtility.UrlDecode(jwtCookies.RefreshToken);
        Assert.NotNull(refreshToken2);

        logger.LogTrace($"2) Wait access token expires ( refreshToken: {refreshToken2} )");

        // old refreshToken still valid because rotated + skew still valid

        await Task.Delay(accessTokenDuration);

        currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

        jwtCookies = currentUserRes.Headers.GetJwtCookiesFromResponse();
        Assert.NotNull(jwtCookies.AccessToken);

        var refreshToken3 = HttpUtility.UrlDecode(jwtCookies.RefreshToken);
        Assert.NotNull(refreshToken3);

        logger.LogTrace($"3) Wait access token expires ( refreshToken: {refreshToken3} )");

        await Task.Delay(accessTokenDuration);

        currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);

        logger.LogTrace("Login");

        loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        jwtCookies = loginRes.Headers.GetJwtCookiesFromResponse();
        Assert.NotNull(jwtCookies.AccessToken);

        var refreshToken4 = HttpUtility.UrlDecode(jwtCookies.RefreshToken);
        Assert.NotNull(refreshToken4);

        // refreshToken4 differs from refreshToken3 because rotate+skew window now expired
        logger.LogTrace($"final refresh token {refreshToken4}");
        Assert.NotEqual(refreshToken4, refreshToken3);

        // the login executed a refresh token maintenance that removed not more valid refreshToken3        
        var refreshTokensCount = dbContext.UserRefreshTokens.Count(w => w.UserName == "admin");
        logger.LogTrace($"refresh tokens in db {refreshTokensCount}");
        Assert.Equal(1, refreshTokensCount);
    }

    /// <summary>
    /// Renewal of refresh token allow to slide the expiration.
    /// </summary>
    [Fact]
    public async Task RewnewRefreshToken()
    {
        var accessTokenDuration = TimeSpan.FromSeconds(1);
        var refreshTokenDuration = TimeSpan.FromSeconds(3);
        var refreshTokenRotationSkew = accessTokenDuration + TimeSpan.FromSeconds(1);

        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var logger = testFactory.Services.GetRequiredService<ILogger<IntegrationTests>>();
        var dbContext = testFactory.Services.GetRequiredService<AppDbContext>();
        var util = testFactory.Services.GetRequiredService<IUtilService>();

        config.SetConfigVar(CONFIG_KEY_JwtSettings_ClockSkewSeconds, "0");

        config.SetConfigVar(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds,
            accessTokenDuration.TotalSeconds.ToString());

        config.SetConfigVar(CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds,
            refreshTokenDuration.TotalSeconds.ToString());

        config.SetConfigVar(CONFIG_KEY_JwtSettings_RefreshTokenRotationSkewSeconds,
            refreshTokenRotationSkew.TotalSeconds.ToString());

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

        logger.LogTrace("Login");

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var loginResObj = await loginRes.DeserializeAsync<LoginResponseDto>(util);
        Assert.NotNull(loginResObj);

        var toWait = loginResObj.RefreshTokenExpiration - DateTimeOffset.UtcNow - TimeSpan.FromSeconds(1);
        logger.LogTrace($"1) Wait ( {toWait.TotalSeconds} sec ) refresh token about to expire");

        await Task.Delay(toWait);

        // refresh token still valid, now slide a new refresh token through renew refresh token

        var renewRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.RenewRefreshToken)}")).ApplySetCookies(client);

        toWait = refreshTokenDuration - TimeSpan.FromSeconds(1);
        logger.LogTrace($"2) Wait ( {toWait.TotalSeconds} sec ) refresh token about to expire");

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
        var logger = adminTestFactory.Services.GetRequiredService<ILogger<IntegrationTests>>();
        var util = adminTestFactory.Services.GetRequiredService<IUtilService>();

        //

        using var userTestFactory = new TestFactory();
        await userTestFactory.InitAsync(dropDb: false);
        var userClient = userTestFactory.Client;
        var userConfig = userTestFactory.Services.GetRequiredService<IConfiguration>();

        userConfig.SetConfigVar(CONFIG_KEY_JwtSettings_ClockSkewSeconds, "0");

        userConfig.SetConfigVar(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds,
            accessTokenDuration.TotalSeconds.ToString());

        userConfig.SetConfigVar(CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds,
            refreshTokenDuration.TotalSeconds.ToString());

        userConfig.SetConfigVar(CONFIG_KEY_JwtSettings_RefreshTokenRotationSkewSeconds,
            refreshTokenRotationSkew.TotalSeconds.ToString());

        //

        var adminUsername = adminConfig.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = adminConfig.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

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
        var logger = testFactory.Services.GetRequiredService<ILogger<IntegrationTests>>();
        var dbContext = testFactory.Services.GetRequiredService<AppDbContext>();

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

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
        var logger = adminTestFactory.Services.GetRequiredService<ILogger<IntegrationTests>>();

        adminConfig.SetConfigVar(CONFIG_KEY_JwtSettings_ClockSkewSeconds, "0");
        adminConfig.SetConfigVar(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds, accessTokenDuration.TotalSeconds.ToString());

        //

        using var userTestFactory = new TestFactory();
        await userTestFactory.InitAsync(dropDb: false);
        var userClient = userTestFactory.Client;
        var userConfig = userTestFactory.Services.GetRequiredService<IConfiguration>();

        userConfig.SetConfigVar(CONFIG_KEY_JwtSettings_ClockSkewSeconds, "0");
        userConfig.SetConfigVar(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds, accessTokenDuration.TotalSeconds.ToString());

        //

        var adminUsername = adminConfig.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = adminConfig.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

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

        var accessTokenDuration = TimeSpan.FromSeconds(config.GetConfigVar<double>(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds));

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        var dtStart = DateTimeOffset.UtcNow;

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var jwtCookies = loginRes.Headers.GetJwtCookiesFromResponse();
        Assert.NotNull(jwtCookies.AccessToken);
        var jwt = DecodeToJwtSecurityToken(jwtCookies.AccessToken);
        Assert.NotNull(jwt);

        // jwt issued >= req start
        Assert.True(jwt.IssuedAt + DRIFT >= dtStart);

        // jet valid from >= req start
        Assert.True(jwt.ValidFrom + DRIFT >= dtStart);

        // jwt from + duration = to
        Assert.Equal(jwt.ValidFrom + accessTokenDuration, jwt.ValidTo);
    }

    /// <summary>
    /// Test access token invalid with fake token.
    /// </summary>
    [Fact]
    public async Task AccessTokenInvalidWithFake()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        var jwtCookies = loginRes.Headers.GetJwtCookiesFromResponse();
        Assert.NotNull(jwtCookies.AccessToken);

        var fakeAccessToken = GenerateFakeAccessToken(config, jwtCookies.AccessToken);

        client.SetCookie(WEB_CookieName_XAccessToken, fakeAccessToken);

        var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.Unauthorized, currentUserRes.StatusCode);
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

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

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

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

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
        Assert.Equal(config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName), listUsers[0].UserName);
        Assert.Equal(config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Email), listUsers[0].Email);
        Assert.Equal(new List<string> { ROLE_admin }, listUsers[0].Roles);
    }

    /// <summary>
    /// Create admin, advanced user, normal user and verify permissions association.
    /// </summary>
    [Fact]
    public async Task VerifyRolePermissionAssociations()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var util = testFactory.Services.GetRequiredService<IUtilService>();

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        // create advanced user

        var advancedUsername = "advancedUsername";
        var advancedEmail = "advanced@test.com";
        var advancedPassword = "advancedPass1!";
        var advancedRoles = new[] { ROLE_advanced };

        var editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            EditUsername = advancedUsername,
            EditEmail = advancedEmail,
            EditPassword = advancedPassword,
            EditRoles = advancedRoles
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

        // create normal user

        var normalUsername = "normalUsername";
        var normalEmail = "normal@test.com";
        var normalPassword = "normalPass1!";
        var normalRoles = new[] { ROLE_normal };

        editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            EditUsername = normalUsername,
            EditEmail = normalEmail,
            EditPassword = normalPassword,
            EditRoles = normalRoles
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

        ExpectedPermissionsNfo[] expectedPermissions = {
            new ExpectedPermissionsNfo(adminUsername, adminPassword,
                PermissionsFromRoles(new HashSet<string>() { ROLE_admin }).ToArray()),

            new ExpectedPermissionsNfo(advancedUsername, advancedPassword,
                PermissionsFromRoles(new HashSet<string>() { ROLE_advanced }).ToArray()),

            new ExpectedPermissionsNfo(normalUsername, normalPassword,
                PermissionsFromRoles(new HashSet<string>() { ROLE_normal }).ToArray()),
        };

        foreach (var permnfo in expectedPermissions)
        {
            // logout
            var logoutRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.Logout)}")).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, logoutRes.StatusCode);

            // login
            loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
            {
                UsernameOrEmail = permnfo.username,
                Password = permnfo.password
            })).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

            // current user
            var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

            var currentUser = await currentUserRes.DeserializeAsync<CurrentUserResponseDto>(util);
            Assert.NotNull(currentUser);
            Assert.Equal(permnfo.permissions, currentUser.Permissions);
        }
    }

    /// <summary>
    /// Test <see cref="AuthController.EditUser"/>
    /// </summary>
    [Fact]
    public async Task TestEditUser()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var util = testFactory.Services.GetRequiredService<IUtilService>();
        var logger = testFactory.Services.GetRequiredService<ILogger<IntegrationTests>>();

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminEmail = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Email);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);
        var adminRoles = new[] { ROLE_admin };

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        // create advanced user

        var advancedUsername = "advancedUsername";
        var advancedEmail = "advanced@test.com";
        var advancedPassword = "advancedPass1!";
        var advancedRoles = new[] { ROLE_advanced };

        var editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            EditUsername = advancedUsername,
            EditEmail = advancedEmail,
            EditPassword = advancedPassword,
            EditRoles = advancedRoles
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

        // create normal user

        var normalUsername = "normalUsername";
        var normalEmail = "normal@test.com";
        var normalPassword = "normalPass1!";
        var normalRoles = new[] { ROLE_normal };

        editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            EditUsername = normalUsername,
            EditEmail = normalEmail,
            EditPassword = normalPassword,
            EditRoles = normalRoles
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

        UserCredentialNfo[] userCredentials = {
            new UserCredentialNfo(adminUsername, adminEmail, adminPassword, "adm"),
            new UserCredentialNfo(advancedUsername, advancedEmail, advancedPassword, "adv"),
            new UserCredentialNfo(normalUsername, normalEmail, normalPassword, "nrm"),
        };

        // other users created by admin
        var otherAdminUsername = $"adm_create_admin";
        var otherAdvancedUsername = $"adm_create_advanced";
        var otherNormalUsername = $"adm_create_normal";

        foreach (var _userCredential in userCredentials.WithIndex())
        {
            var userCredential = _userCredential.item;
            var userCredentialIdx = _userCredential.idx;

            logger.LogTrace($"Testing (1th) {userCredential.username} capabilities");

            // logout
            var logoutRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.Logout)}")).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, logoutRes.StatusCode);

            // login
            loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
            {
                UsernameOrEmail = userCredential.username,
                Password = userCredential.password
            })).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

            // current user
            var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

            var currentUser = await currentUserRes.DeserializeAsync<CurrentUserResponseDto>(util);
            Assert.NotNull(currentUser);

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.CreateAdminUser}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    EditUsername = $"{userCredential.testPrefix}_create_admin",
                    EditEmail = $"{userCredential.testPrefix}_admin@test.com",
                    EditPassword = adminPassword,
                    EditRoles = adminRoles
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.CreateAdminUser))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.CreateAdvancedUser}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    EditUsername = $"{userCredential.testPrefix}_create_advanced",
                    EditEmail = $"{userCredential.testPrefix}_advanced@test.com",
                    EditPassword = advancedPassword,
                    EditRoles = advancedRoles
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.CreateAdvancedUser))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.CreateNormalUser}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    EditUsername = $"{userCredential.testPrefix}_create_normal",
                    EditEmail = $"{userCredential.testPrefix}_normal@test.com",
                    EditPassword = normalPassword,
                    EditRoles = normalRoles
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.CreateNormalUser))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.DisableAdminUser}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherAdminUsername,
                    EditDisabled = true
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.DisableAdminUser))
                {
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    // counterverify

                    var testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherAdminUsername,
                        Password = adminPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.Unauthorized, testLoginRes.StatusCode);

                    // re-enable
                    editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                    {
                        ExistingUsername = otherAdminUsername,
                        EditDisabled = false
                    })).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherAdminUsername,
                        Password = adminPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.OK, testLoginRes.StatusCode);
                }
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.LockoutAdminUser}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherAdminUsername,
                    EditLockoutEnd = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.LockoutAdminUser))
                {
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    // counterverify

                    var testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherAdminUsername,
                        Password = adminPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.Unauthorized, testLoginRes.StatusCode);

                    // re-enable
                    editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                    {
                        ExistingUsername = otherAdminUsername,
                        EditLockoutEnd = DateTimeOffset.UtcNow - TimeSpan.FromDays(1)
                    })).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherAdminUsername,
                        Password = adminPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.OK, testLoginRes.StatusCode);
                }
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.LockoutAdvancedUser}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherAdvancedUsername,
                    EditLockoutEnd = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.LockoutAdvancedUser))
                {
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    // counterverify

                    var testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherAdvancedUsername,
                        Password = advancedPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.Unauthorized, testLoginRes.StatusCode);

                    // re-enable
                    editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                    {
                        ExistingUsername = otherAdvancedUsername,
                        EditLockoutEnd = DateTimeOffset.UtcNow - TimeSpan.FromDays(1)
                    })).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherAdvancedUsername,
                        Password = advancedPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.OK, testLoginRes.StatusCode);
                }
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.LockoutNormalUser}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherNormalUsername,
                    EditLockoutEnd = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.LockoutNormalUser))
                {
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    // counterverify

                    var testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherNormalUsername,
                        Password = normalPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.Unauthorized, testLoginRes.StatusCode);

                    // re-enable
                    editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                    {
                        ExistingUsername = otherNormalUsername,
                        EditLockoutEnd = DateTimeOffset.UtcNow - TimeSpan.FromDays(1)
                    })).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherNormalUsername,
                        Password = normalPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.OK, testLoginRes.StatusCode);
                }
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.ChangeOwnEmail}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = userCredential.username,
                    EditEmail = $"{userCredential.testPrefix}_ownChanged@test.com"
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ChangeOwnEmail))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.ChangeOwnPassword}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = userCredential.username,
                    EditPassword = $"{userCredential.testPrefix}_changedPassword123!"
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ChangeOwnPassword))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.ChangeAdminUserEmail}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherAdminUsername,
                    EditEmail = $"{userCredential.testPrefix}_adminChanged@test.com"
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ChangeAdminUserEmail))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.ChangeAdvancedUserEmail}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherAdvancedUsername,
                    EditEmail = $"{userCredential.testPrefix}_advancedChanged@test.com"
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ChangeAdvancedUserEmail))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.ChangeNormalUserEmail}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherNormalUsername,
                    EditEmail = $"{userCredential.testPrefix}_normalChanged@test.com"
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ChangeNormalUserEmail))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.ChangeUserRoles}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherAdminUsername,
                    EditRoles = [ROLE_advanced, ROLE_normal]
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ChangeUserRoles))
                {
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);

                    // counterverify changes

                    var changedUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.ListUsers)}?username={otherAdminUsername}")).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, changedUserRes.StatusCode);

                    var changeUserList = await changedUserRes.DeserializeAsync<List<UserListItemResponseDto>>(util);

                    Assert.NotNull(changeUserList);
                    Assert.Single(changeUserList);
                    Assert.Equal(
                        (new string[] { ROLE_advanced, ROLE_normal }).OrderBy(w => w),
                        changeUserList[0].Roles.OrderBy(w => w));

                    // reset role to original

                    editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                    {
                        ExistingUsername = otherAdminUsername,
                        EditRoles = [ROLE_admin]
                    })).ApplySetCookies(client);

                    // counterverify changes

                    changedUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.ListUsers)}?username={otherAdminUsername}")).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, changedUserRes.StatusCode);

                    changeUserList = await changedUserRes.DeserializeAsync<List<UserListItemResponseDto>>(util);

                    Assert.NotNull(changeUserList);
                    Assert.Single(changeUserList);
                    Assert.Equal([ROLE_admin], changeUserList[0].Roles);
                }
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

        }

        foreach (var _userCredential in userCredentials.WithIndex())
        {
            var userCredential = _userCredential.item;
            var userCredentialIdx = _userCredential.idx;

            logger.LogTrace($"Testing (2th) {userCredential.username} capabilities");

            // logout
            var logoutRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.Logout)}")).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, logoutRes.StatusCode);

            // login
            loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
            {
                UsernameOrEmail = userCredential.username,
                Password = $"{userCredential.testPrefix}_changedPassword123!"
            })).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

            // current user
            var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

            var currentUser = await currentUserRes.DeserializeAsync<CurrentUserResponseDto>(util);
            Assert.NotNull(currentUser);

            //------------------------------------------------------------------                        
            logger.LogTrace($"  {UserPermission.ResetAdminUserPassword}");
            {

                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherAdminUsername,
                    EditPassword = $"{userCredential.testPrefix}_changedPassword123!"
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ResetAdminUserPassword))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.ResetAdvancedUserPassword}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherAdvancedUsername,
                    EditPassword = $"{userCredential.testPrefix}_changedPassword123!"
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ResetAdvancedUserPassword))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.ResetNormalUserPassword}");
            {
                editUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                {
                    ExistingUsername = otherNormalUsername,
                    EditPassword = $"{userCredential.testPrefix}_changedPassword123!"
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.ResetNormalUserPassword))
                    Assert.Equal(HttpStatusCode.OK, editUserRes.StatusCode);
                else
                    Assert.Equal(HttpStatusCode.Forbidden, editUserRes.StatusCode);
            }

        }

    }

    /// <summary>
    /// Test <see cref="AuthController.DeleteUser(string)"/>
    /// </summary>
    [Fact]
    public async Task TestDeleteUser()
    {
        using var testFactory = new TestFactory();
        await testFactory.InitAsync();
        var client = testFactory.Client;
        var config = testFactory.Services.GetRequiredService<IConfiguration>();
        var util = testFactory.Services.GetRequiredService<IUtilService>();
        var logger = testFactory.Services.GetRequiredService<ILogger<IntegrationTests>>();

        var adminUsername = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminEmail = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Email);
        var adminPassword = config.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_Password);
        var adminRoles = new[] { ROLE_admin };

        var loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
        {
            UsernameOrEmail = adminUsername,
            Password = adminPassword
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

        // create advanced user

        var advancedUsername = "advancedUsername";
        var advancedEmail = "advanced@test.com";
        var advancedPassword = "advancedPass1!";
        var advancedRoles = new[] { ROLE_advanced };

        var deleteUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            EditUsername = advancedUsername,
            EditEmail = advancedEmail,
            EditPassword = advancedPassword,
            EditRoles = advancedRoles
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, deleteUserRes.StatusCode);

        // create normal user

        var normalUsername = "normalUsername";
        var normalEmail = "normal@test.com";
        var normalPassword = "normalPass1!";
        var normalRoles = new[] { ROLE_normal };

        deleteUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
        {
            EditUsername = normalUsername,
            EditEmail = normalEmail,
            EditPassword = normalPassword,
            EditRoles = normalRoles
        })).ApplySetCookies(client);

        Assert.Equal(HttpStatusCode.OK, deleteUserRes.StatusCode);

        UserCredentialNfo[] userCredentials = {
            new UserCredentialNfo(adminUsername, adminEmail, adminPassword, "adm"),
            new UserCredentialNfo(advancedUsername, advancedEmail, advancedPassword, "adv"),
            new UserCredentialNfo(normalUsername, normalEmail, normalPassword, "nrm"),
        };

        foreach (var _userCredential in userCredentials.WithIndex())
        {
            var userCredential = _userCredential.item;
            var userCredentialIdx = _userCredential.idx;

            // other users created by admin
            var otherAdminUsername = $"{userCredential.testPrefix}_adminUser";
            var otherAdvancedUsername = $"{userCredential.testPrefix}_advancedUser";
            var otherNormalUsername = $"{userCredential.testPrefix}_normalUser";

            // create other users
            {
                logger.LogTrace($"Allocate other test users");

                // logout
                var adminLogoutRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.Logout)}")).ApplySetCookies(client);

                Assert.Equal(HttpStatusCode.OK, adminLogoutRes.StatusCode);

                // login
                loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                {
                    UsernameOrEmail = adminUsername,
                    Password = adminPassword
                })).ApplySetCookies(client);

                Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

                //------------------------------------------------------------------
                logger.LogTrace($"  {otherAdminUsername}");
                {
                    deleteUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                    {
                        EditUsername = otherAdminUsername,
                        EditEmail = $"{otherAdminUsername}@test.com",
                        EditPassword = adminPassword,
                        EditRoles = adminRoles
                    })).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, deleteUserRes.StatusCode);
                }

                //------------------------------------------------------------------
                logger.LogTrace($"  {otherAdvancedUsername}");
                {
                    deleteUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                    {
                        EditUsername = otherAdvancedUsername,
                        EditEmail = $"{otherAdvancedUsername}@test.com",
                        EditPassword = advancedPassword,
                        EditRoles = advancedRoles
                    })).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, deleteUserRes.StatusCode);
                }

                //------------------------------------------------------------------
                logger.LogTrace($"  {otherNormalUsername}");
                {
                    deleteUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.EditUser)}/", new EditUserRequestDto
                    {
                        EditUsername = otherNormalUsername,
                        EditEmail = $"{otherNormalUsername}@test.com",
                        EditPassword = normalPassword,
                        EditRoles = normalRoles
                    })).ApplySetCookies(client);

                    Assert.Equal(HttpStatusCode.OK, deleteUserRes.StatusCode);
                }
            }

            logger.LogTrace($"Testing {userCredential.username} Delete capabiilties");

            // logout
            var logoutRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.Logout)}")).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, logoutRes.StatusCode);

            // login
            loginRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
            {
                UsernameOrEmail = userCredential.username,
                Password = userCredential.password
            })).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);

            // current user
            var currentUserRes = (await client.GetAsync($"{AuthApiPrefix}/{nameof(AuthController.CurrentUser)}")).ApplySetCookies(client);

            Assert.Equal(HttpStatusCode.OK, currentUserRes.StatusCode);

            var currentUser = await currentUserRes.DeserializeAsync<CurrentUserResponseDto>(util);
            Assert.NotNull(currentUser);

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.DeleteAdminUser}");
            {
                deleteUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.DeleteUser)}/", new DeleteUserRequestDto
                {
                    UsernameToDelete = otherAdminUsername
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.DeleteAdminUser))
                {
                    Assert.Equal(HttpStatusCode.OK, deleteUserRes.StatusCode);

                    // counterverify

                    var testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherAdminUsername,
                        Password = adminPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.Unauthorized, testLoginRes.StatusCode);
                }
                else
                    Assert.Equal(HttpStatusCode.Forbidden, deleteUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.DeleteAdvancedUser}");
            {
                deleteUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.DeleteUser)}/", new DeleteUserRequestDto
                {
                    UsernameToDelete = otherAdvancedUsername
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.DeleteAdvancedUser))
                {
                    Assert.Equal(HttpStatusCode.OK, deleteUserRes.StatusCode);

                    // counterverify

                    var testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherAdvancedUsername,
                        Password = advancedPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.Unauthorized, testLoginRes.StatusCode);
                }
                else
                    Assert.Equal(HttpStatusCode.Forbidden, deleteUserRes.StatusCode);
            }

            //------------------------------------------------------------------
            logger.LogTrace($"  {UserPermission.DeleteNormalUser}");
            {
                deleteUserRes = (await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.DeleteUser)}/", new DeleteUserRequestDto
                {
                    UsernameToDelete = otherNormalUsername
                })).ApplySetCookies(client);

                if (currentUser.Permissions.Contains(UserPermission.DeleteNormalUser))
                {
                    Assert.Equal(HttpStatusCode.OK, deleteUserRes.StatusCode);

                    // counterverify

                    var testLoginRes = await client.PostAsJsonAsync($"{AuthApiPrefix}/{nameof(AuthController.Login)}", new LoginRequestDto
                    {
                        UsernameOrEmail = otherNormalUsername,
                        Password = normalPassword
                    });/*.ApplySetCookies(client);*/

                    Assert.Equal(HttpStatusCode.Unauthorized, testLoginRes.StatusCode);
                }
                else
                    Assert.Equal(HttpStatusCode.Forbidden, deleteUserRes.StatusCode);
            }
        }

    }

}