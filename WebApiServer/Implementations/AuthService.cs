namespace ExampleWebApp.Backend.WebApi;

public class AuthService : IAuthService
{
    readonly UserManager<ApplicationUser> userManager;
    readonly RoleManager<IdentityRole> roleManager;
    readonly SignInManager<ApplicationUser> signInManager;
    readonly IJWTService jwtService;
    readonly IHttpContextAccessor httpContextAccessor;
    readonly IHostEnvironment environment;
    readonly ILogger<AuthService> logger;
    readonly IConfiguration configuration;
    readonly IAuthenticationService authenticationService;
    readonly IUtilService util;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jwtService,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment environment,
        ILogger<AuthService> logger,
        IConfiguration configuration,
        IAuthenticationService authenticationService,
        IUtilService util
        )
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.signInManager = signInManager;
        this.jwtService = jwtService;
        this.httpContextAccessor = httpContextAccessor;
        this.environment = environment;
        this.logger = logger;
        this.configuration = configuration;
        this.authenticationService = authenticationService;
        this.util = util;
    }

    public AuthOptions AuthOptions()
    {
        var res = new AuthOptions
        {
            Username = new UsernameAuthOptions
            {
                AllowedUserNameCharacters = userManager.Options.User.AllowedUserNameCharacters,
            },

            Password = new PasswordAuthOptions
            {

                RequireDigit = userManager.Options.Password.RequireDigit,
                RequiredLength = userManager.Options.Password.RequiredLength,
                RequiredUniqueChars = userManager.Options.Password.RequiredUniqueChars,
                RequireLowercase = userManager.Options.Password.RequireLowercase,
                RequireNonAlphanumeric = userManager.Options.Password.RequireNonAlphanumeric,
                RequireUppercase = userManager.Options.Password.RequireUppercase,

            }

        };

        return res;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto loginRequestDto,
        CancellationToken cancellationToken)
    {
        ApplicationUser? user;

        if (loginRequestDto.UsernameOrEmail is not null)
        {
            if (loginRequestDto.UsernameOrEmail.Contains("@"))
                user = await userManager.FindByEmailAsync(loginRequestDto.UsernameOrEmail);
            else
                user = await userManager.FindByNameAsync(loginRequestDto.UsernameOrEmail);
        }
        else
            return new LoginResponseDto
            {
                Status = LoginStatus.UsernameOrEmailRequired
            };

        if (user is null)
            return new LoginResponseDto
            {
                Status = LoginStatus.InvalidAuthentication,
                Errors = [$"can't find {loginRequestDto.UsernameOrEmail} user or email"]
            };

        if (user.Disabled == true)
            return new LoginResponseDto
            {
                Status = LoginStatus.InvalidAuthentication,
                Errors = [$"user {user.UserName} disabled"]
            };

        if (loginRequestDto.PasswordResetToken is not null)
        {
            var resetRes = await userManager.ResetPasswordAsync(user, loginRequestDto.PasswordResetToken, loginRequestDto.Password);

            //TODO: check if reset password satisfy password options

            if (!resetRes.Succeeded)
                return new LoginResponseDto
                {
                    Status = LoginStatus.InvalidAuthentication,
                    Errors = [$"reset password toekn invalid for {user.UserName} user"]
                };
        }
        else
        {
            if (!await userManager.CheckPasswordAsync(user, loginRequestDto.Password))
                return new LoginResponseDto
                {
                    Status = LoginStatus.InvalidAuthentication,
                    Errors = [$"check password failed for {user.UserName} user"]
                };
        }

        if (await userManager.IsLockedOutAsync(user))
            return new LoginResponseDto
            {
                Status = LoginStatus.InvalidAuthentication,
                Errors = [$"user {user.UserName} is locked out"]
            };

        var username = user.UserName;
        var email = user.Email;
        var claims = userManager.GetJWTClaims(user);
        if (username is null || email is null)
            throw new Exception("username or email null");

        var accessToken = jwtService.GenerateAccessToken(username, email, claims);
        var refreshToken = jwtService.GenerateRefreshToken(username);

        var persist = false;
        await signInManager.SignInWithClaimsAsync(user, persist, claims);

        var opts = new CookieOptions();

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return new LoginResponseDto
            {
                Status = LoginStatus.InvalidHttpContext
            };

        var userName = user.UserName!;

        environment.SetCookieOptions(configuration, opts, setExpiresAsRefreshToken: true);
        httpContext.Response.Cookies.Append(WEB_CookieName_XAccessToken, accessToken, opts);
        httpContext.Response.Cookies.Append(WEB_CookieName_XRefreshToken, refreshToken, opts);

        return new LoginResponseDto
        {
            Status = LoginStatus.OK,
            UserName = userName,
            Email = user.Email!,
            Roles = claims.GetRoles()
        };
    }

    public async Task<CurrentUserResponseDto> CurrentUserNfoAsync(CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext is null)
            return new CurrentUserResponseDto
            {
                Status = CurrentUserStatus.InvalidArgument
            };

        var quser = httpContextAccessor.HttpContext.User;

        if (quser is not null)
        {
            var userName = quser.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = quser.FindFirstValue(ClaimTypes.Email);

            if (userName is null || email is null)
                return new CurrentUserResponseDto
                {
                    Status = CurrentUserStatus.InvalidAuthentication
                };

            var accessToken = httpContextAccessor.HttpContext.Request.Cookies[WEB_CookieName_XAccessToken];

            if (accessToken is null)
                return new CurrentUserResponseDto
                {
                    Status = CurrentUserStatus.AccessTokenNotFound
                };

            var roles = quser.Claims.GetRoles().ToHashSet();

            return new CurrentUserResponseDto
            {
                Status = CurrentUserStatus.OK,
                UserName = userName,
                Email = email,
                Roles = roles,
                Permissions = PermissionsFromRoles(roles)
            };
        }

        else
            return new CurrentUserResponseDto
            {
                Status = CurrentUserStatus.InvalidAuthentication
            };
    }

    public async Task<HttpStatusCode> LogoutAsync(CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return HttpStatusCode.BadRequest;

        httpContext.Response.Cookies.Delete(WEB_CookieName_XAccessToken);
        httpContext.Response.Cookies.Delete(WEB_CookieName_XRefreshToken);

        await signInManager.SignOutAsync();

        return HttpStatusCode.OK;
    }

    public async Task<List<UserListItemResponseDto>> ListUsersAsync(
        CancellationToken cancellationToken,
        string? username = null)
    {
        var res = new List<UserListItemResponseDto>();

        var q = userManager.Users;

        if (username is not null)
            q = q.Where(r => r.UserName == username);

        var users = await q.ToListAsync();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);

            if (user.UserName is null || user.Email is null)
            {
                logger.LogWarning($"Inconsistent user (username or email null) id:{user.Id}");
                continue;
            }

            res.Add(new UserListItemResponseDto
            {
                Disabled = user.Disabled,
                UserName = user.UserName,
                Email = user.Email,
                AccessFailedCount = user.AccessFailedCount,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = roles,
                TwoFactorEnabled = user.TwoFactorEnabled
            });
        }

        return new List<UserListItemResponseDto>(res);
    }

    async Task<List<string>> AllRolesAsync()
    {
        return (await roleManager.Roles.ToListAsync())
            .Where(w => w.Name != null)
            .Select(w => w.Name!)
            .ToList();
    }

    public async Task<List<string>> ListRolesAsync(CancellationToken cancellationToken)
    {
        var allRoles = await AllRolesAsync();

        if (allRoles is null)
        {
            logger.LogError("There are no roles");
            return new List<string>();
        }

        return new List<string>(allRoles);
    }

    public async Task<DeleteUserResponseDto> DeleteUserAsync(
        string usernameToDelete, CancellationToken cancellationToken)
    {
        var curUserNfo = await CurrentUserNfoAsync(cancellationToken);
        if (curUserNfo.Status != CurrentUserStatus.OK)
            throw new Exception($"can't retrieve current user.");

        var curUser = await userManager.FindByNameAsync(curUserNfo.UserName);
        if (curUser is null)
            throw new Exception($"can't retrieve {curUserNfo.UserName} user.");

        var userToDelete = await userManager.FindByNameAsync(usernameToDelete);
        if (userToDelete is null)
            return new DeleteUserResponseDto
            {
                Status = DeleteUserStatus.UserNotFound
            };

        var editExistingUserRoles = await userManager.GetRolesAsync(userToDelete);
        var editExistingUserMaxRole = MaxRole(editExistingUserRoles ?? []);

        // check if deleting last active admin
        if (editExistingUserMaxRole == ROLE_admin)
        {
            var users = await ListUsersAsync(cancellationToken);
            var q = users.Where(r => r.UserName != usernameToDelete && !r.Disabled).Count();

            if (q == 0)
                return new DeleteUserResponseDto
                {
                    Status = DeleteUserStatus.CannotDeleteLastActiveAdmin
                };
        }

        var hasPermission = false;

        switch (editExistingUserMaxRole)
        {
            case ROLE_admin:
                hasPermission = curUserNfo.Permissions.Contains(UserPermission.DeleteAdminUser);
                break;

            case ROLE_advanced:
                hasPermission = curUserNfo.Permissions.Contains(UserPermission.DeleteAdminUser);
                break;

            case ROLE_normal:
                hasPermission = curUserNfo.Permissions.Contains(UserPermission.DeleteNormalUser);
                break;

            default:
                throw new NotImplementedException($"can't edit other user {userToDelete.UserName} with unknown role {editExistingUserMaxRole}");
        }

        if (!hasPermission)
            return new DeleteUserResponseDto
            {
                Status = DeleteUserStatus.PermissionsError,
                Errors = [$"Can't delete user (role:{editExistingUserMaxRole})."]
            };

        var deleteRes = await userManager.DeleteAsync(userToDelete);
        if (!deleteRes.Succeeded)
            return new DeleteUserResponseDto
            {
                Status = DeleteUserStatus.IdentityError,
                Errors = deleteRes.Errors.Select(w => w.Description).ToList()
            };

        return new DeleteUserResponseDto
        {
            Status = DeleteUserStatus.OK
        };
    }

    public async Task<EditUserResponseDto> EditUserAsync(
        EditUserRequestDto editUserRequestDto, CancellationToken cancellationToken)
    {
        var curUserNfo = await CurrentUserNfoAsync(cancellationToken);
        if (curUserNfo.Status != CurrentUserStatus.OK)
            throw new Exception($"can't retrieve current user");

        var curUser = await userManager.FindByNameAsync(curUserNfo.UserName);
        if (curUser is null)
            throw new Exception($"can't retrieve {curUserNfo.UserName} user.");

        var res = new EditUserResponseDto
        {
            Status = EditUserStatus.OK
        };

        //
        // new user
        //
        if (string.IsNullOrWhiteSpace(editUserRequestDto.ExistingUsername))
        {
            if (editUserRequestDto.EditRoles is null || editUserRequestDto.EditRoles.Count == 0)
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.PermissionsError,
                    Errors = ["Can't create user with no roles."]
                };

            if (!curUserNfo.Permissions.Contains(UserPermission.CreateAdminUser) &&
                !curUserNfo.Permissions.Contains(UserPermission.CreateAdvancedUser) &&
                !curUserNfo.Permissions.Contains(UserPermission.CreateNormalUser))
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.PermissionsError,
                    Errors = ["Can't create any user."]
                };

            var reqCreateAdmin = (editUserRequestDto.EditRoles ?? []).Contains(ROLE_admin);
            var reqCreateAdv = (editUserRequestDto.EditRoles ?? []).Contains(ROLE_advanced);
            var reqCreateNormal = (editUserRequestDto.EditRoles ?? []).Contains(ROLE_normal);

            if (reqCreateAdmin && !curUserNfo.Permissions.Contains(UserPermission.CreateAdminUser))
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.PermissionsError,
                    Errors = ["Can't create admin user."]
                };

            if (reqCreateAdv && !curUserNfo.Permissions.Contains(UserPermission.CreateAdvancedUser))
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.PermissionsError,
                    Errors = ["Can't create advanced user."]
                };

            if (reqCreateNormal && !curUserNfo.Permissions.Contains(UserPermission.CreateNormalUser))
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.PermissionsError,
                    Errors = ["Can't create normal user."]
                };

            var user = new ApplicationUser
            {
                UserName = editUserRequestDto.EditUsername,
                Email = editUserRequestDto.EditEmail
            };

            if (editUserRequestDto.EditPassword is null) throw new ArgumentNullException(nameof(editUserRequestDto.EditPassword));

            var createRes = await userManager.CreateAsync(user, editUserRequestDto.EditPassword);
            if (!createRes.Succeeded)
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.IdentityError,
                    Errors = createRes.Errors.Select(w => w.Description).ToList()
                };

            if (editUserRequestDto.EditRoles is not null && editUserRequestDto.EditRoles.Count > 0)
            {
                var roleRes = await userManager.AddToRolesAsync(user, editUserRequestDto.EditRoles);
                if (!roleRes.Succeeded)
                    return new EditUserResponseDto
                    {
                        Status = EditUserStatus.IdentityError,
                        Errors = createRes.Errors.Select(w => w.Description).ToList()
                    };
            }
        }

        //
        // existing user
        //
        else
        {
            var editExistingUser = await userManager.FindByNameAsync(editUserRequestDto.ExistingUsername);
            if (editExistingUser is null)
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.UserNotFound
                };

            if (editUserRequestDto.EditUsername is not null)
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.CannotChangeUsername
                };

            //---------------------------------------
            // edit lockout
            //---------------------------------------
            if (editUserRequestDto.EditLockoutEnd is not null)
            {
                var editExistingUserRoles = await userManager.GetRolesAsync(editExistingUser);
                var editExistingUserMaxRole = MaxRole(editExistingUserRoles ?? []);

                if (editExistingUserMaxRole is null)
                    throw new InternalError($"Can't edit other user {editExistingUser.UserName} that has no roles");

                var hasPermission = false;

                switch (editExistingUserMaxRole)
                {
                    case ROLE_admin:
                        hasPermission = curUserNfo.Permissions.Contains(UserPermission.LockoutAdminUser);
                        break;

                    case ROLE_advanced:
                        hasPermission = curUserNfo.Permissions.Contains(UserPermission.LockoutAdvancedUser);
                        break;

                    case ROLE_normal:
                        hasPermission = curUserNfo.Permissions.Contains(UserPermission.LockoutNormalUser);
                        break;

                    default:
                        throw new NotImplementedException($"can't edit other user {editExistingUser.UserName} with unknown role {editExistingUserMaxRole}");
                }

                if (!hasPermission)
                    return new EditUserResponseDto
                    {
                        Status = EditUserStatus.PermissionsError,
                        Errors = [$"Can't edit other user (role:{editExistingUserMaxRole}) lockout."]
                    };

                var lockoutRes = await userManager.SetLockoutEndDateAsync(editExistingUser, editUserRequestDto.EditLockoutEnd);

                if (!lockoutRes.Succeeded)
                    return new EditUserResponseDto
                    {
                        Status = EditUserStatus.IdentityError,
                        Errors = lockoutRes.Errors.Select(w => w.Description).ToList()
                    };
            }

            //---------------------------------------
            // edit disabled
            //---------------------------------------
            if (editUserRequestDto.EditDisabled is not null && editExistingUser.Disabled != editUserRequestDto.EditDisabled)
            {
                var editExistingUserRoles = await userManager.GetRolesAsync(editExistingUser);
                var editExistingUserMaxRole = MaxRole(editExistingUserRoles ?? []);

                if (editExistingUserMaxRole is null)
                    throw new InternalError($"Can't edit other user {editExistingUser.UserName} that has no roles");

                var hasPermission = false;

                switch (editExistingUserMaxRole)
                {
                    case ROLE_admin:
                        hasPermission = curUserNfo.Permissions.Contains(UserPermission.DisableAdminUser);
                        break;

                    case ROLE_advanced:
                        hasPermission = curUserNfo.Permissions.Contains(UserPermission.DisableAdvancedUser);
                        break;

                    case ROLE_normal:
                        hasPermission = curUserNfo.Permissions.Contains(UserPermission.DisableNormalUser);
                        break;

                    default:
                        throw new NotImplementedException($"can't disable user {editExistingUser.UserName} with unknown role {editExistingUserMaxRole}");
                }

                if (!hasPermission)
                    return new EditUserResponseDto
                    {
                        Status = EditUserStatus.PermissionsError,
                        Errors = [$"Can't disable user (role:{editExistingUserMaxRole})."]
                    };

                editExistingUser.Disabled = editUserRequestDto.EditDisabled == true;

                var updateRes = await userManager.UpdateAsync(editExistingUser);

                if (!updateRes.Succeeded)
                    return new EditUserResponseDto
                    {
                        Status = EditUserStatus.IdentityError,
                        Errors = updateRes.Errors.Select(w => w.Description).ToList()
                    };
            }

            //---------------------------------------
            // edit roles
            //---------------------------------------
            if (editUserRequestDto.EditRoles is not null)
            {
                if (!curUserNfo.Permissions.Contains(UserPermission.ChangeUserRoles))
                    return new EditUserResponseDto
                    {
                        Status = EditUserStatus.PermissionsError,
                        Errors = ["Can't edit user roles."]
                    };

                var allRoles = await AllRolesAsync();
                var userRoles = await userManager.GetRolesAsync(editExistingUser);
                var userRolesToSet = editUserRequestDto.EditRoles.Where(allRoles.Contains).ToList();

                if (userRoles is not null)
                {
                    var rolesToAdd = userRolesToSet.Where(roleToSet => !userRoles.Contains(roleToSet)).ToList();
                    var rolesToRemove = userRoles.Where(userRole => !userRolesToSet.Contains(userRole)).ToList();

                    if (rolesToAdd.Count > 0)
                        await userManager.AddToRolesAsync(editExistingUser, rolesToAdd);

                    if (rolesToRemove.Count > 0)
                        await userManager.RemoveFromRolesAsync(editExistingUser, rolesToRemove);

                    res.RolesAdded = rolesToAdd.ToList();
                    res.RolesRemoved = rolesToRemove.ToList();
                }

                else
                {
                    return new EditUserResponseDto
                    {
                        Status = EditUserStatus.PermissionsError,
                        Errors = ["Can't edit user with no roles."]
                    };
                }
            }

            //---------------------------------------
            // edit email
            //---------------------------------------
            if (editUserRequestDto.EditEmail is not null)
            {
                ApplicationUser? userToModify = null;

                //
                // edit itself email
                //
                if (editUserRequestDto.ExistingUsername == curUserNfo.UserName &&
                    curUserNfo.Email != editUserRequestDto.EditEmail)
                {
                    if (!curUserNfo.Permissions.Contains(UserPermission.ChangeOwnEmail))
                        return new EditUserResponseDto
                        {
                            Status = EditUserStatus.PermissionsError,
                            Errors = ["Can't edit own email."]
                        };

                    userToModify = curUser;
                }

                //
                // edit other user email
                //
                else if (editExistingUser.Email != editUserRequestDto.EditEmail)
                {
                    var editExistingUserRoles = await userManager.GetRolesAsync(editExistingUser);
                    var editExistingUserMaxRole = MaxRole(editExistingUserRoles ?? []);

                    if (editExistingUserMaxRole is null)
                        throw new InternalError($"Can't edit other user {editExistingUser.UserName} that has no roles");

                    var hasPermission = false;

                    switch (editExistingUserMaxRole)
                    {
                        case ROLE_admin:
                            hasPermission = curUserNfo.Permissions.Contains(UserPermission.ChangeAdminUserEmail);
                            break;

                        case ROLE_advanced:
                            hasPermission = curUserNfo.Permissions.Contains(UserPermission.ChangeAdvancedUserEmail);
                            break;

                        case ROLE_normal:
                            hasPermission = curUserNfo.Permissions.Contains(UserPermission.ChangeNormalUserEmail);
                            break;

                        default:
                            throw new NotImplementedException($"can't edit other user {editExistingUser.UserName} with unknown role {editExistingUserMaxRole}");
                    }

                    if (!hasPermission)
                        return new EditUserResponseDto
                        {
                            Status = EditUserStatus.PermissionsError,
                            Errors = [$"Can't edit other user (role:{editExistingUserMaxRole}) email."]
                        };

                    userToModify = editExistingUser;
                }

                if (userToModify is not null)
                {
                    var token = await userManager.GenerateChangeEmailTokenAsync(userToModify, editUserRequestDto.EditEmail);
                    if (token is null)
                        throw new Exception($"Can't retrieve change email token for {userToModify.UserName} user.");

                    var changeEmailRes = await userManager.ChangeEmailAsync(userToModify, editUserRequestDto.EditEmail, token);
                    if (!changeEmailRes.Succeeded)
                        return new EditUserResponseDto
                        {
                            Status = EditUserStatus.IdentityError,
                            Errors = changeEmailRes.Errors.Select(w => w.Description).ToList()
                        };
                }
            }

            //---------------------------------------
            // edit password
            //---------------------------------------
            if (editUserRequestDto.EditPassword is not null)
            {
                ApplicationUser? userToModify = null;

                //
                // edit itself password
                //
                if (editUserRequestDto.ExistingUsername == curUserNfo.UserName)
                {
                    if (!curUserNfo.Permissions.Contains(UserPermission.ChangeOwnPassword))
                        return new EditUserResponseDto
                        {
                            Status = EditUserStatus.PermissionsError,
                            Errors = ["Can't edit own password."]
                        };

                    userToModify = curUser;
                }

                //
                // edit other user password
                //
                else
                {
                    var editExistingUserRoles = await userManager.GetRolesAsync(editExistingUser);
                    var editExistingUserMaxRole = MaxRole(editExistingUserRoles ?? []);

                    if (editExistingUserMaxRole is null)
                        throw new InternalError($"Can't edit other user {editExistingUser.UserName} that has no roles");

                    var hasPermission = false;

                    switch (editExistingUserMaxRole)
                    {
                        case ROLE_admin:
                            hasPermission = curUserNfo.Permissions.Contains(UserPermission.ResetAdminUserPassword);
                            break;

                        case ROLE_advanced:
                            hasPermission = curUserNfo.Permissions.Contains(UserPermission.ResetAdvancedUserPassword);
                            break;

                        case ROLE_normal:
                            hasPermission = curUserNfo.Permissions.Contains(UserPermission.ResetNormalUserPassword);
                            break;

                        default:
                            throw new NotImplementedException($"can't edit other user {editExistingUser.UserName} with unknown role {editExistingUserMaxRole}");
                    }

                    if (!hasPermission)
                        return new EditUserResponseDto
                        {
                            Status = EditUserStatus.PermissionsError,
                            Errors = [$"Can't edit other user (role:{editExistingUserMaxRole}) password."]
                        };

                    userToModify = editExistingUser;
                }

                if (userToModify is not null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(userToModify);
                    if (token is null)
                        throw new Exception($"Can't retrieve reset password token for {userToModify.UserName} user.");

                    var resetPasswordRes = await userManager.ResetPasswordAsync(userToModify, token, editUserRequestDto.EditPassword);
                    if (!resetPasswordRes.Succeeded)
                        return new EditUserResponseDto
                        {
                            Status = EditUserStatus.IdentityError,
                            Errors = resetPasswordRes.Errors.Select(w => w.Description).ToList()
                        };
                }

            }

        }

        return res;
    }

    public async Task<ResetLostPasswordResponseDto> ResetLostPasswordRequestAsync(
        string email, string? token, string? resetPassword, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return new ResetLostPasswordResponseDto
            {
                Status = ResetLostPasswordStatus.NotFound
            };

        if (token is not null && resetPassword is not null)
        {
            var loginRes = await LoginAsync(new LoginRequestDto
            {
                UsernameOrEmail = email,
                Password = resetPassword,
                PasswordResetToken = token
            }, cancellationToken);

            if (loginRes.Status == LoginStatus.OK)
                return new ResetLostPasswordResponseDto
                {
                    Status = ResetLostPasswordStatus.OK
                };

            return new ResetLostPasswordResponseDto
            {
                Status = ResetLostPasswordStatus.InvalidToken
            };
        }

        var emailServerUsername = configuration.GetConfigVar<string>(CONFIG_KEY_EmailServer_Username);
        var emailServerPassword = configuration.GetConfigVar<string>(CONFIG_KEY_EmailServer_Password);
        var emailServerSmtpServerName = configuration.GetConfigVar<string>(CONFIG_KEY_EmailServer_SmtpServer);
        var emailServerSmtpServerPort = configuration.GetConfigVar<int>(CONFIG_KEY_EmailServer_SmtpServerPort);
        var emailServerSecurityOption = configuration.GetConfigVar<ConfigValuesEmailServerSecurity>(CONFIG_KEY_EmailServer_Security);

        var emailServerFromDisplayName = configuration.GetConfigVar<string?>(CONFIG_KEY_EmailServer_FromDisplayName) ?? "Server";

        var appServerName = configuration.GetConfigVar<string>(CONFIG_KEY_AppServerName);

        var pwToken = await userManager.GeneratePasswordResetTokenAsync(user);

        var resetUrl = $"https://{appServerName}/app/Login/:from/{HttpUtility.UrlEncode(pwToken)}";

        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(emailServerFromDisplayName, emailServerUsername));
        msg.To.Add(new MailboxAddress(email, email));

        msg.Body = new TextPart("html")
        {
            Text = $$"""
            <a href="{{resetUrl}}">Click here</a> to reset your password.
            """
        };

        using (var client = new SmtpClient())
        {
            switch (emailServerSecurityOption)
            {
                case ConfigValuesEmailServerSecurity.None:
                    await client.ConnectAsync(
                        emailServerSmtpServerName,
                        emailServerSmtpServerPort,
                        MailKit.Security.SecureSocketOptions.None,
                        cancellationToken);
                    break;

                case ConfigValuesEmailServerSecurity.Auto:
                    await client.ConnectAsync(
                        emailServerSmtpServerName,
                        emailServerSmtpServerPort,
                        MailKit.Security.SecureSocketOptions.Auto,
                        cancellationToken);
                    break;

                case ConfigValuesEmailServerSecurity.Ssl:
                    await client.ConnectAsync(
                        emailServerSmtpServerName,
                        emailServerSmtpServerPort,
                        MailKit.Security.SecureSocketOptions.SslOnConnect,
                        cancellationToken);
                    break;

                case ConfigValuesEmailServerSecurity.Tls:
                    await client.ConnectAsync(
                        emailServerSmtpServerName,
                        emailServerSmtpServerPort,
                        MailKit.Security.SecureSocketOptions.StartTls,
                        cancellationToken);
                    break;

                default: throw new NotImplementedException($"unknown email server security option {emailServerSecurityOption}");
            }

            await client.AuthenticateAsync(
                new NetworkCredential(emailServerUsername, emailServerPassword),
                cancellationToken);

            await client.SendAsync(msg, cancellationToken);

            await client.DisconnectAsync(quit: true, cancellationToken);
        }

        return new ResetLostPasswordResponseDto
        {
            Status = ResetLostPasswordStatus.OK
        };
    }

}
