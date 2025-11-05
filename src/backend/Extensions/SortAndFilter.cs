namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    public static IQueryable<T> ApplySortAndFilter<T>(this IQueryable<T> q, GenericSort? sort, string? dynFilter)
    {
        if (sort is not null)
        {
            int idx = 0;
            foreach (var col in sort.Columns)
            {
                var pascalColName = col.ColumnName.ToPascalCase();

                var testOrder = q.OrderBySmart(pascalColName, col.Direction, addictive: idx > 0);

                if (testOrder is not null)
                    q = testOrder;

                ++idx;
            }
        }

        if (!string.IsNullOrWhiteSpace(dynFilter))
        {            
            q = q.Where(dynFilter);
        }

        return q;
    }

}
