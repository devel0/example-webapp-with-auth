namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Fake;

public interface IFakeService
{

    /// <summary>
    /// count items with given optiona filtering
    /// </summary>
    public Task<int> CountAsync(string? dynFilter, CancellationToken cancellationToken);

    /// <summary>
    /// get items with optional filtering, sorting
    /// </summary>     
    public Task<List<FakeData>> GetViewAsync(
        int off, int cnt, string? dynFilter, GenericSort? sort, CancellationToken cancellationToken
    );

    /// <summary>
    /// get record by id
    /// </summary>
    Task<FakeData?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// delete record by id
    /// </summary>
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// update given record        
    /// </summary>    
    Task<FakeData> UpdateAsync(GenericItemWithOrig<FakeData> location, CancellationToken cancellationToken);

}