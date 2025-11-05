namespace ExampleWebApp.Backend.WebApi;

[ApiController]
[Authorize]
[Route($"{API_PREFIX}/[controller]/[action]")]
public class FakeDataController : ControllerBase
{
    readonly CancellationToken cancellationToken;
    readonly ILogger logger;
    readonly IFakeService fakeService;

    public FakeDataController(
        CancellationToken cancellationToken,
        ILogger<FakeDataController> logger,
        IFakeService fakeService
        )
    {
        this.cancellationToken = cancellationToken;
        this.logger = logger;
        this.fakeService = fakeService;
    }

    /// <summary>
    /// get items with optional filtering, sorting
    /// </summary>        
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<FakeData>>> GetFakeDatas(GetGenericRequest req) =>
        await fakeService.GetViewAsync(req.Offset, req.Count, req.DynFilter, req.Sort, cancellationToken);

    /// <summary>
    /// get record by id
    /// </summary>
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<FakeData?>> GetFakeDataById(Guid id) =>
        await fakeService.GetByIdAsync(id, cancellationToken);

    /// <summary>
    /// delete record by id
    /// </summary>
    [HttpDelete]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult> DeleteFakeData(Guid id)
    {
        if (!await fakeService.DeleteByIdAsync(id, cancellationToken))
            return NotFound();

        return Ok();
    }

    /// <summary>
    /// update given record
    /// </summary>
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<FakeData?>> UpdateFakeData(GenericItemWithOrig<FakeData> req) =>
        await fakeService.UpdateAsync(req, cancellationToken);

}