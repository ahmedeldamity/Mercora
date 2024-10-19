namespace BlazorEcommerce.Persistence;
public class GenericRepository<T> (StoreContext storeContext) : IGenericRepository<T> where T : BaseEntity
{
    public async Task<IReadOnlyList<T>> GetAllAsync() => await storeContext.Set<T>().ToListAsync();

    public async Task<IReadOnlyList<T>> GetAllAsync(ISpecifications<T> spec)
        => await SpecificationsEvaluator<T>.GetQuery(storeContext.Set<T>(), spec).ToListAsync();

    public async Task<int> GetCountAsync(ISpecifications<T> spec) 
        => await SpecificationsEvaluator<T>.GetQuery(storeContext.Set<T>(), spec).CountAsync();

    public async Task<T?> GetEntityAsync(int id) => await storeContext.Set<T>().FindAsync(id);

    public async Task<T?> GetEntityAsync(ISpecifications<T> spec)
        => await SpecificationsEvaluator<T>.GetQuery(storeContext.Set<T>(), spec).FirstOrDefaultAsync();

    public async Task AddAsync(T entity) => await storeContext.Set<T>().AddAsync(entity);

    public void Delete(T entity) => storeContext.Set<T>().Remove(entity);
    
    public void Update(T entity) => storeContext.Set<T>().Update(entity);
}