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
    /// Retrieve auth options.
    /// </summary>    
    [HttpGet]
    public ActionResult<AuthOptions> AuthOptions() => Ok(authService.AuthOptions());

    /// <summary>
    /// Retrieve current logged in user name, email, roles.
    /// </summary>    
    [HttpGet]
    public ActionResult<CurrentUserResponseDto> CurrentUser()
    {
        var res = authService.CurrentUserNfo();

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

    [HttpPost]
    public async Task<ActionResult<RenewAccessTokenResponse>> RenewAccessToken()
    {
        var res = await authService.RenewCurrentUserAccessTokenAsync(cancellationToken);

        switch (res.Status)
        {
            case RenewAccessTokenStatus.OK:
                return res;

            case RenewAccessTokenStatus.InvalidAuthentication:
            case RenewAccessTokenStatus.InvalidAccessToken:
            case RenewAccessTokenStatus.InvalidRefreshToken:
                return Unauthorized();

            case RenewAccessTokenStatus.InvalidHttpContext:
                return BadRequest();

            default:
                throw new NotImplementedException($"{nameof(RenewAccessTokenStatus)} == {res}");
        }
    }

    /// <summary>
    /// Renew refresh token of current user if refresh token still valid.
    /// This is used to extends refresh token duration avoiding closing frontend session.
    /// </summary>    
    [HttpPost]
    public async Task<ActionResult<RenewRefreshTokenResponse>> RenewRefreshToken()
    {
        var res = await authService.RenewCurrentUserRefreshTokenAsync(cancellationToken);

        switch (res.Status)
        {
            case RenewRefreshTokenStatus.OK:
                return res;

            case RenewRefreshTokenStatus.AccessTokenNotFound:
            case RenewRefreshTokenStatus.InvalidAuthentication:
            case RenewRefreshTokenStatus.InvalidRefreshToken:
                return Unauthorized();

            case RenewRefreshTokenStatus.InvalidHttpContext:
                return BadRequest();

            default:
                throw new NotImplementedException($"{nameof(RenewRefreshTokenStatus)} == {res}");
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
    [Authorize(Roles = $"{ROLE_admin},{ROLE_advanced}")]
    public async Task<ActionResult<List<UserListItemResponseDto>>> ListUsers(
        string? username = null
    )
    {
        var res = await authService.ListUsersAsync(cancellationToken, username);

        return res;
    }

    /// <summary>
    /// count items with optional filtering
    /// </summary>        
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<int>> CountUsers(CountGenericRequest req) =>
        await authService.CountAsync(req.dynFilter, cancellationToken);

    /// <summary>
    /// get items with optional filtering, sorting
    /// </summary>          
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<UserListItemResponseDto>>> GetUsers(GetGenericRequest req) =>
        await authService.GetViewAsync(req.Offset, req.Count, req.DynFilter, req.Sort, cancellationToken);

    /// <summary>
    /// List all roles.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = $"{ROLE_admin},{ROLE_advanced}")]
    public async Task<ActionResult<List<string>>> ListRoles()
    {
        var res = await authService.ListRolesAsync(cancellationToken);

        return res;
    }

    /// <summary>
    /// Delete user.
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = $"{ROLE_admin},{ROLE_advanced},{ROLE_normal}")]
    public async Task<ActionResult<DeleteUserResponseDto>> DeleteUser(
        DeleteUserRequestDto deleteUserReq)
    {
        var res = await authService.DeleteUserAsync(deleteUserReq.UsernameToDelete, cancellationToken);

        switch (res.Status)
        {
            case DeleteUserStatus.OK:
                return res;

            case DeleteUserStatus.UserNotFound:
                return NotFound();

            case DeleteUserStatus.IdentityError:
            case DeleteUserStatus.PermissionsError:
            case DeleteUserStatus.CannotDeleteLastActiveAdmin:
                return Problem(
                    title: $"{res.Status}",
                    detail: string.Join(';', res.Errors),
                    statusCode: (int)HttpStatusCode.Forbidden);

            default:
                throw new NotImplementedException($"{nameof(DeleteUserResponseDto)}.{nameof(DeleteUserResponseDto.Status)} == {res.Status}");
        }

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
            case EditUserStatus.CannotChangeUsername:
                return Problem(
                    title: $"{res.Status}",
                    detail: string.Join(';', res.Errors),
                    statusCode: (int)HttpStatusCode.Forbidden);

            default:
                throw new NotImplementedException($"{nameof(EditUserResponseDto)}.{nameof(EditUserResponseDto.Status)} == {res.Status}");
        }

    }

    /// <summary>
    /// Reset lost password.
    /// </summary>    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> ResetLostPassword(string? email, string? token, string? resetPassword)
    {
        if (email is null)
            throw new BadHttpRequestException("Email address required.");

        var res = await authService.ResetLostPasswordRequestAsync(
            email,
            token,
            resetPassword,
            cancellationToken);

        switch (res.Status)
        {
            case ResetLostPasswordStatus.OK:
                return Ok();

            case ResetLostPasswordStatus.EmailServerError:
                return Problem(
                    title: $"{res.Status}",
                    detail: "Email server error.",
                    statusCode: (int)HttpStatusCode.InternalServerError
                    );

            case ResetLostPasswordStatus.NotFound:
                return NotFound();

            default:
                throw new NotImplementedException($"{nameof(ResetLostPasswordResponseDto)}.{nameof(ResetLostPasswordResponseDto.Status)} == {res.Status}");
        }
    }

}
