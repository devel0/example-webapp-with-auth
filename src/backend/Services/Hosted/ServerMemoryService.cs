namespace ExampleWebApp.Backend.WebApi.Services;

public class ServerMemoryService : IWorkerBase
{
    readonly ILogger logger;
    readonly IWebSocketService<ExampleWSProtocol> wsService;

    public TimeSpan ExecInterval => TimeSpan.FromSeconds(30);

    public ServerMemoryService(
        ILogger<ServerMemoryService> logger,
        IWebSocketService<ExampleWSProtocol> wsService
    )
    {
        this.logger = logger;
        this.wsService = wsService;
    }

    public async Task ExecAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("EXEC mem serv");
        await this.wsService.SendToAllClientsAsync(new ExampleWSProtoServerMem
        {
            MemoryUsed = GC.GetTotalMemory(forceFullCollection: false)
        }, skipDuplicates: true, cancellationToken);

    }

}