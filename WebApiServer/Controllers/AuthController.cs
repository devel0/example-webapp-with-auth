namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Authorize]
[Route($"{API_PREFIX}/[controller]/[action]")]
public class AuthController : ControllerBase
{

    readonly IAuthService authService;
    readonly CancellationToken cancellationToken;
    readonly ILogger logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger,
        CancellationToken cancellationToken
        )
    {
        this.authService = authService;
        this.logger = logger;
        this.cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Retrieve current logged in user name, email, roles.
    /// </summary>    
    [HttpGet]
    public async Task<ActionResult<CurrentUserResponseDto>> CurrentUser()
    {
        var res = await authService.CurrentUserNfoAsync(cancellationToken);

        switch (res.Status)
        {
            case CurrentUserStatus.OK:
                return res;

            case CurrentUserStatus.AccessTokenNotFound:
            case CurrentUserStatus.InvalidAuthentication:
                return Unauthorized();

            case CurrentUserStatus.InvalidArgument:
                return BadRequest();

            default:
                throw new NotImplementedException($"{nameof(CurrentUserResponseDto)}.{nameof(CurrentUserResponseDto.Status)} == {res.Status}");
        }
    }

    /// <summary>
    /// Login user by given username or email and auth password.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto loginRequestDto)
    {
        var res = await authService.LoginAsync(loginRequestDto, cancellationToken);

        switch (res.Status)
        {
            case LoginStatus.OK:
                return res;

            case LoginStatus.InvalidAuthentication:
            case LoginStatus.UsernameOrEmailRequired:
                {
                    // logger.LogTrace($"Unauth reason {string.Join(';', res.Errors)}");
                    return Unauthorized();
                }

            case LoginStatus.InvalidHttpContext:
                return BadRequest();

            default:
                throw new NotImplementedException($"{nameof(LoginResponseDto)}.{nameof(LoginResponseDto.Status)} == {res.Status}");
        }
    }

    /// <summary>
    /// Logout current user.
    /// </summary>    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        var res = await authService.LogoutAsync(cancellationToken);

        return StatusCode((int)res);
    }

    /// <summary>
    /// List all users or specific if param given.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<UserListItemResponseDto>>> ListUsers(
        string? username = null
    )
    {
        var res = await authService.ListUsersAsync(cancellationToken, username);

        return res;
    }

    /// <summary>
    /// List all roles.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<string>>> ListRoles()
    {
        var res = await authService.ListRolesAsync(cancellationToken);

        return res;
    }

    /// <summary>
    /// Edit user data
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = $"{ROLE_admin},{ROLE_advanced},{ROLE_normal}")]
    public async Task<ActionResult<EditUserResponseDto>> EditUser(EditUserRequestDto editUser)
    {
        var res = await authService.EditUserAsync(editUser, cancellationToken);

        switch (res.Status)
        {
            case EditUserStatus.OK:
                return res;

            case EditUserStatus.UserNotFound:
                return NotFound();

            case EditUserStatus.IdentityError:
            case EditUserStatus.PermissionsError:
                return Problem(
                    string.Join(';', res.Errors),
                    statusCode: (int)HttpStatusCode.Forbidden);

            default:
                throw new NotImplementedException($"{nameof(EditUserResponseDto)}.{nameof(EditUserResponseDto.Status)} == {res.Status}");
        }

    }

}
