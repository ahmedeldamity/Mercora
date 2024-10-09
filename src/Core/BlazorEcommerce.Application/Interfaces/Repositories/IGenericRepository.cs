using BlazorEcommerce.Application.Interfaces.Specifications;
using BlazorEcommerce.Domain.Common;

namespace BlazorEcommerce.Application.Interfaces.Repositories;
public interface IGenericRepository<T> where T : BaseEntity
{
    public Task<IReadOnlyList<T>> GetAllAsync();
    public Task<IReadOnlyList<T>> GetAllAsync(ISpecifications<T> spec);
    Task<int> GetCountAsync(ISpecifications<T> spec);

    public Task<T?> GetEntityAsync(ISpecifications<T> spec);
    public Task<T?> GetEntityAsync(int id);
    public Task AddAsync(T entity);
    public void Update(T entity);
    public void Delete(T entity);
}