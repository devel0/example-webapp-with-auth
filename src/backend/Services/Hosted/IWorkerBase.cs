namespace ExampleWebApp.Backend.WebApi.Services;

public interface IWorkerBase
{

    TimeSpan ExecInterval { get; }

    Task ExecAsync(CancellationToken cancellationToken);

}