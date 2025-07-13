namespace ChatappLC.Application.Common;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortField, bool isDescending)
    {
        if (string.IsNullOrEmpty(sortField)) return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, sortField);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = isDescending ? "OrderByDescending" : "OrderBy";

        var method = typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
    }

    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, Func<IQueryable<T>, IQueryable<T>> searchLogic)
    {
        return searchLogic(query);
    }
}
