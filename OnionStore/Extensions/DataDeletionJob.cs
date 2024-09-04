using Microsoft.EntityFrameworkCore;
using Repository.Store;

namespace API.Extensions;
public class DataDeletionJob(StoreContext identityContext)
{
    private readonly StoreContext _identityContext = identityContext;

    public async Task Execute()
    {
        await _identityContext.IdentityCodes
        .Where(p => p.IsActive == false && p.CreationTime < DateTime.UtcNow.AddMinutes(-5))
        .ForEachAsync(p => _identityContext.IdentityCodes.Remove(p));

        await _identityContext.SaveChangesAsync();
    }

}