using Core.Common;
using Core.Interfaces.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Repository;
public class SpecificationsEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
    {
        var query = inputQuery;

        query = query.Where(spec.WhereCriteria);

        query = query.OrderBy(spec.OrderBy);

        if (spec.IsPaginationEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        query = spec.IncludesCriteria.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

        return query;
    }
}