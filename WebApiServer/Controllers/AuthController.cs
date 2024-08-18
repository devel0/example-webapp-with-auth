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
    readonly CancellationToken cancellationToken;

    public AuthController(
        IAuthService authService,
        CancellationToken cancellationToken
        )
    {
        this.authService = authService;
        this.cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Retrieve current logged in user name, email, roles.
    /// </summary>    
    [HttpGet]
    public async Task<ActionResult<CurrentUserResponseDto>> CurrentUser()
    {
        var res = await authService.CurrentUserAsync(cancellationToken);

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
    /// Immediate user lockout until given time or unlock if time is in the past ( UTC ).
    /// Note that this happens when access token expires.
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<IActionResult> LockoutUser(
        [FromBody] LockoutUserRequestDto lockoutUserRequestDto)
    {
        var res = await authService.LockoutUserAsync(lockoutUserRequestDto, cancellationToken);

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
        var res = await authService.LoginAsync(loginRequestDto, cancellationToken);

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
    /// Change user roles
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<SetUserRolesResponseDto>> SetUserRoles(SetUserRolesRequestDto setUserRolesRequestDto)
    {
        var res = await authService.SetUserRolesAsync(setUserRolesRequestDto, cancellationToken);

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


    /// <summary>
    /// Create user by given username, email, password.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<RegisterUserResponseDto>> RegisterUser(
        [FromBody] RegisterUserRequestDto registerUserRequestDto)
    {
        var res = await authService.RegisterUserAsync(registerUserRequestDto, cancellationToken);

        switch (res.Status)
        {
            case RegisterUserStatus.IdentityError:
                return BadRequest();
        }

        return res;
    }

    /// <summary>
    /// Edit user data
    /// </summary>    
    [HttpPost]
    public async Task<ActionResult> EditUser(EditUserRequestDto editUser)
    {        
        var res = await authService.EditUserAsync(editUser, cancellationToken);                

        switch (res.Status)
        {
            case EditUserStatus.AdminRolesReadOnly:
                return Problem("admin roles are readonly");

            case EditUserStatus.InternalError:
                return StatusCode((int)HttpStatusCode.InternalServerError, res.Errors);

            case EditUserStatus.IdentityError:            
                return Forbid();            

            case EditUserStatus.InvalidPassword:
                return Problem("Invalid password");

            case EditUserStatus.UserNotFound:
                return NotFound();
        }

        return Ok();
    }

}
