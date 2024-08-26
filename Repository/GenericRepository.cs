using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Specifications;
using Microsoft.EntityFrameworkCore;
using Repository.Store;

namespace Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _storeContext;

        public GenericRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _storeContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(ISpecifications<T> spec)
        {
            return await SpecificationsEvaluator<T>.GetQuery(_storeContext.Set<T>(), spec).ToListAsync();
        }

        public async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
            return await SpecificationsEvaluator<T>.GetQuery(_storeContext.Set<T>(), spec).CountAsync();
        }

        public async Task<T?> GetEntityAsync(int id)
        {
            return await _storeContext.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetEntityAsync(ISpecifications<T> spec)
        {
            return await SpecificationsEvaluator<T>.GetQuery(_storeContext.Set<T>(), spec).FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _storeContext.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _storeContext.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _storeContext.Set<T>().Update(entity);
        }
    }
}