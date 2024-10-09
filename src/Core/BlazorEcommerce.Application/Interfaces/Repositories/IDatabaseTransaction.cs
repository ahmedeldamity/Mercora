namespace BlazorEcommerce.Application.Interfaces.Repositories;
public interface IDatabaseTransaction: IDisposable
{
	public Task BeginTransactionAsync();
	public Task CommitTransactionAsync();
	public Task RollbackTransactionAsync();
}