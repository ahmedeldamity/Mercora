using System.Linq.Expressions;
using BlazorEcommerce.Domain.Common;

namespace BlazorEcommerce.Application.Interfaces.Specifications;
public interface ISpecifications<T> where T : BaseEntity
{
    public Expression<Func<T, bool>> WhereCriteria { get; set; }
    public List<Expression<Func<T, object>>> IncludesCriteria { get; set; }
    public Expression<Func<T, object>> OrderBy { get; set; }
    public Expression<Func<T, object>> OrderByDesc { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public bool IsPaginationEnabled { get; set; }
}