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

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jwtService,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment environment,
        ILogger<AuthService> logger,
        IConfiguration configuration,
        IAuthenticationService authenticationService)
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
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto loginRequestDto,
        CancellationToken cancellationToken)
    {
        ApplicationUser? user = null;

        if (loginRequestDto.UserName is not null)
            user = await userManager.FindByNameAsync(loginRequestDto.UserName);
        else
        {
            if (loginRequestDto.Email is null)
                return new LoginResponseDto
                {
                    Status = LoginStatus.UsernameOrEmailRequired
                };

            user = await userManager.FindByEmailAsync(loginRequestDto.Email);
        }

        if (user is null || !await userManager.CheckPasswordAsync(user, loginRequestDto.Password))
            return new LoginResponseDto
            {
                Status = LoginStatus.InvalidAuthentication
            };

        var username = user.UserName;
        var email = user.Email;
        var claims = userManager.GetJWTClaims(user);
        if (username is null || email is null)
            throw new Exception("username or email null");

        var accessToken = jwtService.GenerateAccessToken(username, email, claims);
        var refreshToken = jwtService.GetValidRefreshToken(username);

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
        httpContext.Response.Cookies.Append(WEB_CookieName_XUsername, userName, opts);
        httpContext.Response.Cookies.Append(WEB_CookieName_XRefreshToken, refreshToken, opts);

        return new LoginResponseDto
        {
            Status = LoginStatus.OK,
            UserName = userName,
            Email = user.Email!,
            Roles = claims.GetRoles()
        };
    }

    public async Task<HttpStatusCode> LockoutUserAsync(LockoutUserRequestDto lockoutUserRequestDto,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByNameAsync(lockoutUserRequestDto.UserName);
        if (user is not null)
        {

            logger.LogInformation($"User [{user}] locked out until {lockoutUserRequestDto.LockoutEnd}");
            await userManager.SetLockoutEndDateAsync(user, lockoutUserRequestDto.LockoutEnd);

            return HttpStatusCode.OK;
        }

        else
            return HttpStatusCode.BadRequest;
    }

    public async Task<CurrentUserResponseDto> CurrentUserAsync(CancellationToken cancellationToken)
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

            // TODO: modularize cookie management
            var accessToken = httpContextAccessor.HttpContext.Request.Cookies[WEB_CookieName_XAccessToken];

            if (accessToken is null)
                return new CurrentUserResponseDto
                {
                    Status = CurrentUserStatus.AccessTokenNotFound
                };

            return new CurrentUserResponseDto
            {
                Status = CurrentUserStatus.OK,
                UserName = userName,
                Email = email,
                Roles = quser.Claims.GetRoles()
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
        httpContext.Response.Cookies.Delete(WEB_CookieName_XUsername);
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
                UserName = user.UserName,
                Email = user.Email,
                AccessFailedCount = user.AccessFailedCount,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnd = user.LockoutEnd,
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

    public async Task<SetUserRolesResponseDto> SetUserRolesAsync(SetUserRolesRequestDto setUserRolesRequestDto,
        CancellationToken cancellationToken = default)
    {
        if (setUserRolesRequestDto.UserName == configuration.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName))
        {
            logger.LogWarning($"Can't change admin roles");
            return new SetUserRolesResponseDto
            {
                Status = SetUserRolesStatus.AdminRolesReadOnly
            };
        }

        var user = await userManager.FindByNameAsync(setUserRolesRequestDto.UserName);

        if (user is null)
        {
            logger.LogWarning($"Can't find user [{setUserRolesRequestDto.UserName}]");
            return new SetUserRolesResponseDto
            {
                Status = SetUserRolesStatus.UserNotFound
            };
        }

        var allRoles = await AllRolesAsync();
        var userRoles = await userManager.GetRolesAsync(user);
        var userRolesToSet = setUserRolesRequestDto.Roles.Where(roleToSet => allRoles.Contains(roleToSet)).ToList();

        if (userRoles is not null)
        {
            var rolesToAdd = userRolesToSet.Where(roleToSet => !userRoles.Contains(roleToSet));
            var rolesToRemove = userRoles.Where(userRole => !userRolesToSet.Contains(userRole));

            await userManager.AddToRolesAsync(user, rolesToAdd);
            await userManager.RemoveFromRolesAsync(user, rolesToRemove);

            logger.LogInformation(
                $"Added [{string.Join(',', rolesToAdd)}] roles ; " +
                $"Removed [{string.Join(',', rolesToRemove)}] roles ; " +
                $"username:{setUserRolesRequestDto.UserName}");

            return new SetUserRolesResponseDto
            {
                Status = SetUserRolesStatus.OK,
                RolesAdded = rolesToAdd.ToList(),
                RolesRemoved = rolesToRemove.ToList()
            };
        }

        else
        {
            logger.LogError($"There are no roles for user {setUserRolesRequestDto.UserName}");
            return new SetUserRolesResponseDto
            {
                Status = SetUserRolesStatus.InternalError
            };
        }
    }

    public async Task<RegisterUserResponseDto> RegisterUserAsync(
       RegisterUserRequestDto registerUserRequestDto,
       CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = registerUserRequestDto.UserName,
            Email = registerUserRequestDto.Email
        };

        var createRes = await userManager.CreateAsync(user, registerUserRequestDto.Password);
        if (createRes.Succeeded)
        {
            return new RegisterUserResponseDto
            {
                Status = RegisterUserStatus.OK,
                Errors = new List<IdentityError>()
            };
        }
        else
        {
            var status = RegisterUserStatus.IdentityError;

            return new RegisterUserResponseDto
            {
                Status = status,
                Errors = createRes.Errors.ToList()
            };
        }
    }

    public async Task<EditUserResponseDto> EditUserAsync(
        EditUserRequestDto editUserRequestDto, CancellationToken cancellationToken)
    {
        if (editUserRequestDto.CreateNew)
        {
            if (editUserRequestDto.ChangePassword is null)
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.InvalidPassword
                };

            var registerRes = await RegisterUserAsync(new RegisterUserRequestDto
            {
                UserName = editUserRequestDto.UserName,
                Email = editUserRequestDto.Email,
                Password = editUserRequestDto.ChangePassword
            }, cancellationToken);

            if (registerRes.Status != RegisterUserStatus.OK)
            {
                return new EditUserResponseDto
                {
                    Status = registerRes.Status switch
                    {
                        RegisterUserStatus.IdentityError => EditUserStatus.IdentityError,
                        _ => throw new NotImplementedException($"unhandled status {registerRes.Status}")
                    },
                    Errors = registerRes.Errors.Select(w => w.ToString() ?? "").ToList()
                };
            }
        }

        var user = await userManager.FindByNameAsync(editUserRequestDto.UserName);

        if (user is null)
        {
            return new EditUserResponseDto
            {
                Status = EditUserStatus.UserNotFound
            };
        }

        if (user.UserName != editUserRequestDto.UserName)
        {
            var editUsernameRes = await userManager.SetUserNameAsync(user, editUserRequestDto.UserName);
            if (!editUsernameRes.Succeeded)
                return new EditUserResponseDto
                {
                    Status = EditUserStatus.IdentityError,
                    Errors = editUsernameRes.Errors.Select(w => w.ToString() ?? "").ToList()
                };
        }

        {
            var changeRolesRes = await SetUserRolesAsync(new SetUserRolesRequestDto
            {
                UserName = editUserRequestDto.UserName,
                Roles = editUserRequestDto.Roles
            }, cancellationToken);

            if (changeRolesRes.Status != SetUserRolesStatus.OK)
            {
                return new EditUserResponseDto
                {
                    Status = changeRolesRes.Status switch
                    {
                        SetUserRolesStatus.AdminRolesReadOnly => EditUserStatus.AdminRolesReadOnly,
                        SetUserRolesStatus.InternalError => EditUserStatus.InternalError,
                        SetUserRolesStatus.UserNotFound => EditUserStatus.UserNotFound,
                        _ => throw new NotImplementedException($"unhandled status {changeRolesRes.Status}")
                    }
                };
            }
        }

        return new EditUserResponseDto
        {
            Status = EditUserStatus.OK
        };
    }

}
