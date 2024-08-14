using Core.Entities;
using Core.Interfaces.Repositories;
using Repository.Store;
using System.Collections.Concurrent;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _storeContext;
        private readonly ConcurrentDictionary<string, object> _repositories;

        public UnitOfWork(StoreContext storeContext)
        {
            _storeContext = storeContext;
            _repositories = new ConcurrentDictionary<string, object>();
        }

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var key = typeof(T).Name;

            return (IGenericRepository<T>)_repositories.GetOrAdd(key, _ => new GenericRepository<T>(_storeContext));
        }

        public async Task<int> CompleteAsync()
        {
            return await _storeContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _storeContext.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await _storeContext.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}