using Core.Entities;
using Core.Interfaces.Specifications;

namespace Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        public Task<IReadOnlyList<T>> GetAllAsync();
        public Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
        Task<int> GetCountAsync(ISpecifications<T> spec);

        public Task<T?> GetByIdWithSpecAsync(ISpecifications<T> spec);
        public Task<T?> GetByIdAsync(int id);

        // These will using in MVC dashboard
        public Task AddAsync(T entity);
        public void UpdateAsync(T entity);
        public void DeleteAsync(T entity);
    }
}
