using Core.Common;
using Core.Interfaces.Repositories;
using Repository.Store;
using System.Collections.Concurrent;

namespace Repository;
public class UnitOfWork(StoreContext storeContext) : IUnitOfWork
{
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var key = typeof(T).Name;

        return (IGenericRepository<T>)_repositories.GetOrAdd(key, _ => new GenericRepository<T>(storeContext));
    }

    public async Task<int> CompleteAsync() => await storeContext.SaveChangesAsync();

    public void Dispose() => storeContext.DisposeAsync();

    public async ValueTask DisposeAsync() => await storeContext.DisposeAsync();
}