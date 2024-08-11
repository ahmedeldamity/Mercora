using Core.Entities;
using Core.Interfaces.Specifications;
using System.Linq.Expressions;

namespace Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> WhereCriteria { get; set; }
        public List<Expression<Func<T, object>>> IncludesCriteria { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
    }
}