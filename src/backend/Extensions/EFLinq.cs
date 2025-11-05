namespace ExampleWebApp.Backend.WebApi;

public class DynLinq
{

    public static Expression<Func<T, string>> KeySelector<T>(string key)
    {
        var entityType = typeof(T);

        var parameter = Expression.Parameter(entityType, "x");

        var property = Expression.Property(parameter, key);

        var body = Expression.Convert(property, typeof(string));

        var lamba = Expression.Lambda<Func<T, string>>(body, parameter);

        return lamba;
    }

}

public static partial class Extensions
{

    public static IQueryable<T> FilterEquals<T>(this IQueryable<T> query, string propertyName, object value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(value);
        var converted = Expression.Convert(constant, property.Type);
        var equal = Expression.Equal(property, converted);
        var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

        return query.Where(lambda);
    }

    public static IQueryable<T> FilterToLowerContains<T>(this IQueryable<T> query, string propertyName, string value)
    {
        // Get the type of the entity
        var entityType = typeof(T);

        // Create a parameter expression for the entity
        var parameter = Expression.Parameter(entityType, "e");

        // Get the property to filter on
        var property = Expression.Property(parameter, propertyName);

        // Convert the property to string and apply ToLower
        var toLowerCall = Expression.Call(property, "ToLower", Type.EmptyTypes);

        var constant = Expression.Constant(value.ToLower());

        Expression expr;

        // if (exactMatch)
        // {
        //     expr = Expression.Equal(toLowerCall, constant);
        // }

        // else
        {
            // Create the Contains method call
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            expr = Expression.Call(toLowerCall, containsMethod, constant);
        }

        // Create the lambda expression
        var lambda = Expression.Lambda<Func<T, bool>>(expr, parameter);

        // Apply the filter to the query
        return query.Where(lambda);
    }

    static Type tGuid = typeof(Guid);
    static Type tString = typeof(String);

    public static IQueryable<T> MultiFilterToLowerContains<T>(this IQueryable<T> query, string value, bool exactMatch)
    {
        // Get the type of the entity
        var entityType = typeof(T);

        // Create a parameter expression for the entity
        var parameter = Expression.Parameter(entityType, "e");

        var calls = new List<Expression>();

        Expression<Func<T, bool>>? f = null;

        foreach (var prop in entityType.GetProperties())
        {
            // Get the property to filter on
            var property = Expression.Property(parameter, prop.Name);

            if (property.Type != tString) continue;

            // Convert the property to string and apply ToLower
            var toLowerCall = Expression.Call(property, "ToLower", Type.EmptyTypes);

            var constant = Expression.Constant(value.ToLower());

            Expression expr;

            if (exactMatch)
            {
                expr = Expression.Equal(toLowerCall, constant);
            }

            else
            {
                // Create the Contains method call
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                expr = Expression.Call(toLowerCall, containsMethod, constant);
            }

            calls.Add(expr);
        }

        if (calls.Count == 0) return query;

        f = Expression.Lambda<Func<T, bool>>(calls[0], parameter);

        if (calls.Count > 1)
        {
            Expression? expr = null;

            Expression BuildOr(IEnumerable<Expression> expressions)
            {
                if (expressions.Count() == 1) return expressions.First();

                return Expression.OrElse(expressions.First(), BuildOr(expressions.Skip(1)));
            }

            f = Expression.Lambda<Func<T, bool>>(BuildOr(calls), parameter);
        }

        // Apply the filter to the query
        return query.Where(f);
    }

    /// <summary>
    /// order by and order by descending depending on given dir.
    /// </summary>
    /// <param name="addictive">if true then template assumed as IOrderedQueryable</param>
    public static IOrderedQueryable<T>? OrderBySmart<T>(this IQueryable<T> source,
        string propertyName, SortDirection dir, bool addictive = false)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = dir switch
        {
            SortDirection.Ascending => addictive ? "ThenBy" : "OrderBy",
            SortDirection.Descending => addictive ? "ThenByDescending" : "OrderByDescending",
            _ => throw new ArgumentException($"unknown sort dir {dir}")
        };

        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IOrderedQueryable<T>?)method.Invoke(null, [source, lambda]);
    }

}
