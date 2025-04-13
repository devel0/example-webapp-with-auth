namespace ExampleWebApp.Backend.Test.Basic;

public class TestRoles
  : IClassFixture<WebApplicationFactory<Program>>
{
    readonly ITestOutputHelper testOutput;

    public TestRoles(
        ITestOutputHelper testOutputHelper
        )
    {
        testOutput = testOutputHelper;
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

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;

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
        var logger = testFactory.Services.GetRequiredService<ILogger<TestBasicAuth>>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminPassword = adminUserConfig.Password;
        var adminEmail = adminUserConfig.Email;
        var adminRoles = adminUserConfig.Roles;

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
        var logger = testFactory.Services.GetRequiredService<ILogger<TestBasicAuth>>();

        var adminUserConfig = config.GetAdminUserConfig();
        var adminUsername = adminUserConfig.Username;
        var adminEmail = adminUserConfig.Email;
        var adminPassword = adminUserConfig.Password;
        var adminRoles = adminUserConfig.Roles;

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