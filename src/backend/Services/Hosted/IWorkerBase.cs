namespace ExampleWebApp.Backend.WebApi.Services;

public interface IWorkerBase
{
    TimeSpan StartupDelay { get; }

    TimeSpan ExecInterval { get; }

    Task ExecAsync(CancellationToken cancellationToken);

}