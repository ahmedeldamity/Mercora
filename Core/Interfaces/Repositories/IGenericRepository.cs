using Core.Entities;

namespace Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        public Task<IReadOnlyList<T>> GetAllAsync();
        public Task<T?> GetByIdAsync(int id);

        // These will using in MVC dashboard
        public Task AddAsync(T entity);
        public void UpdateAsync(T entity);
        public void DeleteAsync(T entity);
    }
}
