namespace BlazorEcommerce.Persistence;

public class EntityDatabaseTransaction(StoreContext storeContext) : IDatabaseTransaction
{
	public async Task BeginTransactionAsync()
	{
		await storeContext.Database.BeginTransactionAsync();
	}

	public async Task CommitTransactionAsync()
	{
		await storeContext.Database.CommitTransactionAsync();
	}

	public async Task RollbackTransactionAsync()
	{
		await storeContext.Database.RollbackTransactionAsync();
	}

	public void Dispose()
	{
		storeContext.Dispose();
	}

}