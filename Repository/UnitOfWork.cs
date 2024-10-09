using BlazorEcommerce.Application.Interfaces.Repositories;
using BlazorEcommerce.Domain.Common;
using BlazorEcommerce.Persistence.Store;
using System.Collections.Concurrent;

namespace BlazorEcommerce.Persistence;
public class UnitOfWork(StoreContext storeContext) : IUnitOfWork
{
    private readonly ConcurrentDictionary<string, object> _repositories = new();

	public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var key = typeof(T).Name;

        return (IGenericRepository<T>)_repositories.GetOrAdd(key, _ => new GenericRepository<T>(storeContext));
    }

	public IDatabaseTransaction BeginTransaction()
	{
		return new EntityDatabaseTransaction(storeContext);
	}

	public async Task<int> CompleteAsync() => await storeContext.SaveChangesAsync();

    public void Dispose() => storeContext.DisposeAsync();

    public async ValueTask DisposeAsync() => await storeContext.DisposeAsync();
}