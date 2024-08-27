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
    /// Long running api test.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult> LongRunning()
    {
        logger.LogDebug("Started long running op");

        await Task.Delay(1000, cancellationToken);

        logger.LogDebug("Finished long running op");

        return Ok();
    }

    /// <summary>
    /// Generate test exception.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult> TestException()
    {
        throw new Exception("Test exception message.");
    }

}