namespace ExampleWebApp.Backend.WebApi.Services;

public class WorkerMainService : BackgroundService
{

    readonly IServiceScopeFactory scopeFactory;

    public WorkerMainService(
        IServiceScopeFactory scopeFactory
        )
    {
        this.scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WorkerMainService>>();
            var sp = scope.ServiceProvider;

            List<IWorkerBase> workers = new();

            var assembly = Assembly.GetAssembly(typeof(WorkerMainService));
            if (assembly is not null)
            {
                var types = assembly.GetTypes().Where(r => r.IsClass && !r.IsAbstract && typeof(IWorkerBase).IsAssignableFrom(r)).ToList();
                foreach (var type in types)
                {
                    var worker = sp.GetRequiredService(type) as IWorkerBase;
                    if (worker is not null)
                        workers.Add(worker);
                }
            }

            if (workers.Count == 0)
            {
                logger.LogInformation($"no workers {nameof(IWorkerBase)} found.");
                return;
            }

            var dt = DateTimeOffset.UtcNow;
            var maintenanceMinInterval = WORKER_THROTTLE_INTERVAL;

            Dictionary<IWorkerBase, DateTimeOffset> lastExecDict = new();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var worker in workers)
                    {
                        var run = false;
                        if (!lastExecDict.ContainsKey(worker))                        
                            run = true;
                        
                        else
                        {
                            var lastExec = lastExecDict[worker];
                            var qTs = DateTimeOffset.UtcNow - lastExec;
                            run = qTs >= worker.ExecInterval;                            
                        }

                        if (run)
                        {
                            lastExecDict[worker] = DateTimeOffset.UtcNow;
                            await worker.ExecAsync(cancellationToken);
                        }
                    }

                    /// avoid cpu fill by throttling at <see cref="maintenanceMinInterval"/>
                    {
                        var timeDiff = DateTimeOffset.UtcNow - dt;
                        if (timeDiff < maintenanceMinInterval)
                        {
                            var towait = maintenanceMinInterval - timeDiff;
                            if (towait.TotalSeconds > 0)
                                await Task.Delay(towait, cancellationToken);
                        }
                    }

                    dt = DateTimeOffset.UtcNow;
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation($"Worker stopped");
                }
            }

        }
    }

}