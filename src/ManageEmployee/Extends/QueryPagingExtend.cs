using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Extends;

public static class QueryPagingExtend
{
    public static IQueryable<T> QueryDate<T>(this IQueryable<T> query, PagingRequestFilterDateModel param)
         where T : BaseEntityCommon
    {
        if (param.FromAt != null)
        {
            var dtFrom = new DateTime(param.FromAt.Value.Year, param.FromAt.Value.Month, param.FromAt.Value.Day);
            query = query.Where(x => x.CreatedAt >= dtFrom);
        }
        if (param.ToAt != null)
        {
            var dtTo = new DateTime(param.ToAt.Value.Year, param.ToAt.Value.Month, param.ToAt.Value.Day);
            dtTo = dtTo.AddDays(1);
            query = query.Where(x => x.CreatedAt <= dtTo);
        }

        return query;
    }
    public static IQueryable<T> QuerySearchTextProcedure<T>(this IQueryable<T> query, ProcedurePagingRequestModel param)
         where T : BaseProcedureEntityCommon
    {
        if (!string.IsNullOrEmpty(param.SearchText))
        {
            query = query.Where(x => x.Code.Contains(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));
        }
        if (param.UserId != null)
        {
            query = query.Where(x => x.UserCreated == param.UserId);
        }
        return query;
    }
}