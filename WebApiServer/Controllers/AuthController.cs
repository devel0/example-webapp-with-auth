namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Authorize]
[Route($"{API_BASE_URL}/[controller]/[action]")]
public class AuthController : ControllerBase
{

    readonly IAuthService authService;

    public AuthController(
        IAuthService authService
        )
    {
        this.authService = authService;
    }

    /// <summary>
    /// Retrieve current logged in user name, email, roles.
    /// </summary>    
    [HttpGet]
    public async Task<ActionResult<CurrentUserResponseDto>> CurrentUser()
    {
        var res = await authService.CurrentUserAsync();

        switch (res.Status)
        {
            case CurrentUserStatus.AccessTokenNotFound:
            case CurrentUserStatus.InvalidAuthentication:
                return Unauthorized();

            case CurrentUserStatus.InvalidArgument:
                return BadRequest();
        }

        return res;
    }

    /// <summary>
    /// Create user by given username, email, password.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<RegisterUserResponseDto>> RegisterUser(
        [FromBody] RegisterUserRequestDto registerUserRequestDto)
    {
        var res = await authService.RegisterUserAsync(registerUserRequestDto);

        switch (res.Status)
        {
            case RegisterUserStatus.IdentityError:
                return BadRequest();
        }

        return res;
    }

    /// <summary>
    /// Immediate user lockout until given time or unlock if time is in the past ( UTC ).
    /// Note that this happens when access token expires.
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<IActionResult> LockoutUser(
        [FromBody] LockoutUserRequestDto lockoutUserRequestDto)
    {
        var res = await authService.LockoutUserAsync(lockoutUserRequestDto);

        return StatusCode((int)res);
    }

    /// <summary>
    /// Login user by given username or email and auth password.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto loginRequestDto)
    {
        var res = await authService.LoginAsync(loginRequestDto);

        switch (res.Status)
        {
            case LoginStatus.InvalidAuthentication:
            case LoginStatus.UsernameOrEmailRequired:
                return Unauthorized();

            case LoginStatus.InvalidHttpContext:
                return BadRequest();
        }

        return res;
    }

    /// <summary>
    /// Logout current user.
    /// </summary>    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        var res = await authService.LogoutAsync();

        return StatusCode((int)res);
    }

    /// <summary>
    /// List all users.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<UserListItemResponseDto>>> ListUsers()
    {
        var res = await authService.ListUsersAsync();

        return res;
    }

    /// <summary>
    /// List all roles.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<string>>> ListRoles()
    {
        var res = await authService.ListRolesAsync();

        return res;
    }

    /// <summary>
    /// Change user roles
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<SetUserRolesResponseDto>> SetUserRoles(SetUserRolesRequestDto setUserRolesRequestDto)
    {
        var res = await authService.SetUserRolesAsync(setUserRolesRequestDto);

        switch (res.Status)
        {
            case SetUserRolesStatus.AdminRolesReadOnly:
                return Forbid();

            case SetUserRolesStatus.InternalError:
                return StatusCode((int)HttpStatusCode.InternalServerError);

            case SetUserRolesStatus.UserNotFound:
                return NotFound();
        }

        return res;
    }

}
