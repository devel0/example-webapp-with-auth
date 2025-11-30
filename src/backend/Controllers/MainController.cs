namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Main app controller
/// </summary>
[ApiController]
[Authorize]
[Route($"{API_PREFIX}/[controller]/[action]")]
public class MainController : ControllerBase
{
    readonly CancellationToken cancellationToken;
    readonly ILogger logger;

    public MainController(
        CancellationToken cancellationToken,
        ILogger<MainController> logger
        )
    {
        this.cancellationToken = cancellationToken;
        this.logger = logger;
    }

    /// <summary>
    /// Long running api test. ( admin and users allowed )
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = $"{ROLE_admin},{ROLE_normal}")]
    public async Task<ActionResult> LongRunning()
    {
        logger.LogDebug("Started long running op");

        await Task.Delay(1000, cancellationToken);

        logger.LogDebug("Finished long running op");

        return Ok();
    }

    /// <summary>
    /// Generate test exception. ( only admin )
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public Task<ActionResult> TestException()
    {
        throw new Exception("Test exception message.");
    }

    //---------------------------- websocket

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> AliveWebSocket(
        IHttpContextAccessor httpContextAccessor
    )
    {
        var context = httpContextAccessor.HttpContext;

        if (context is not null)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var wsClientManager = context.RequestServices.GetRequiredService<IWebSocketService<ExampleWSProtocol>>();
                await wsClientManager.HandleAsync(context, cancellationToken);

                return new EmptyResult();
            }

            else
                return StatusCode((int)HttpStatusCode.UpgradeRequired);
        }

        return Ok();
    }

}