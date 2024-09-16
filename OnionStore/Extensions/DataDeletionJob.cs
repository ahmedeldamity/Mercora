using Microsoft.EntityFrameworkCore;
using Repository.Store;

namespace API.Extensions;
public class DataDeletionJob(StoreContext identityContext)
{
    public async Task Execute()
    {
        await identityContext.IdentityCodes
        .Where(p => p.IsActive == false && p.CreationTime < DateTime.UtcNow.AddMinutes(-5))
        .ForEachAsync(p => identityContext.IdentityCodes.Remove(p));

        await identityContext.SaveChangesAsync();
    }
}