namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.DTOs;

public class ColumnFilter
{

    public string columnName { get; set; }

    public string Filter { get; set; }

} 

public enum SortDirection { Ascending, Descending };

public class SortModelItem
{

    public string ColumnName { get; set; }

    public SortDirection Direction { get; set; }

    public override string ToString() => $"{ColumnName} {Direction}";

}

public class GenericSort
{
    public List<SortModelItem> Columns { get; set; } = new();

}

/// <summary>
/// get request
/// </summary>
/// <param name="Offset">paged from</param>
/// <param name="Count">paged size (-1 disabled)</param>
/// <param name="DynFilter">ef core dynamic filter</param>
/// <param name="Sort"></param>
public record GetGenericRequest(int Offset, int Count, string? DynFilter, GenericSort? Sort);

/// <summary>
/// count request
/// </summary>
public record CountGenericRequest(string? dynFilter);

//[SwaggerSchema(Required = [])] // makes all properties as optional
public class GenericItemWithOrig<T>
{

    public T? origItem { get; set; }

    public required T updatedItem { get; set; }

}