using BlazorEcommerce.Domain.Common;

namespace BlazorEcommerce.Application.Interfaces.Repositories;
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
	public IGenericRepository<T> Repository<T>() where T : BaseEntity;
    public Task<int> CompleteAsync();
	public IDatabaseTransaction BeginTransaction();
}