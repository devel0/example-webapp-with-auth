namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Authorize]
[Route($"{API_BASE_URL}/[controller]/[action]")]
public class AuthController : ControllerBase
{

    readonly ILogger<AuthController> logger;
    readonly IAuthService authService;
    readonly IHttpContextAccessor httpContextAccessor;
    readonly IHostEnvironment environment;

    public AuthController(
        ILogger<AuthController> logger,
        IAuthService authService,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment environment)
    {
        this.logger = logger;
        this.authService = authService;
        this.httpContextAccessor = httpContextAccessor;
        this.environment = environment;        
    }

    /// <summary>
    /// Retrieve current logged in user name, email, roles.
    /// </summary>    
    [HttpGet]
    public async Task<ActionResult<CurrentUserResponseDto>> CurrentUser() =>
        await authService.CurrentUserAsync();

    /// <summary>
    /// Create user by given username, email, password.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<RegisterUserResponseDto>> RegisterUser(
        [FromBody] RegisterUserRequestDto registerUserRequestDto) =>
        await authService.RegisterUserAsync(registerUserRequestDto);

    /// <summary>
    /// Immediate user lockout until given time or unlock if time is in the past ( UTC ).
    /// Note that this happens when access token expires.
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<IActionResult> LockoutUser(
        [FromBody] LockoutUserRequestDto lockoutUserRequestDto) =>
        StatusCode((int)await authService.LockoutUserAsync(lockoutUserRequestDto));

    /// <summary>
    /// Login user by given username or email and auth password.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto loginRequestDto) =>
        await authService.LoginAsync(loginRequestDto);

    /// <summary>
    /// Logout current user.
    /// </summary>    
    [HttpGet]
    public async Task<IActionResult> Logout() =>
        StatusCode((int)await authService.LogoutAsync());

    /// <summary>
    /// List all users.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<UserListItemResponseDto>>> ListUsers() =>
        await authService.ListUsersAsync();

    /// <summary>
    /// List all roles.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<string>>> ListRoles() =>
        await authService.ListRolesAsync();

    /// <summary>
    /// Change user roles
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<SetUserRolesResponseDto>> SetUserRoles(SetUserRolesRequestDto setUserRolesRequestDto) =>
        await authService.SetUserRolesAsync(setUserRolesRequestDto);

}
