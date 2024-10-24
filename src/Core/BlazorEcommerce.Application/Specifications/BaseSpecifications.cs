﻿namespace BlazorEcommerce.Application.Specifications;
public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
{
    public Expression<Func<T, bool>> WhereCriteria { get; set; } = null!;
    public List<Expression<Func<T, object>>> IncludesCriteria { get; set; } = [];
    public Expression<Func<T, object>> OrderBy { get; set; } = null!;
    public Expression<Func<T, object>> OrderByDesc { get; set; } = null!;
    public int Skip { get; set; }
    public int Take { get; set; }
    public bool IsPaginationEnabled { get; set; }

    public void ApplyPagination(int skip, int take)
    {
        IsPaginationEnabled = true;
        Skip = skip;
        Take = take;
    }
}