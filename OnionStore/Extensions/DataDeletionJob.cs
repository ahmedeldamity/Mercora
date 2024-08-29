using Microsoft.EntityFrameworkCore;
using Repository.Identity;

namespace API.Extensions
{
    public class DataDeletionJob(IdentityContext identityContext)
    {
        private readonly IdentityContext _identityContext = identityContext;

        public async Task Execute()
        {
            await _identityContext.IdentityCodes
            .Where(p => p.IsActive == false && p.CreationTime < DateTime.UtcNow.AddMinutes(-5))
            .ForEachAsync(p => _identityContext.IdentityCodes.Remove(p));

            await _identityContext.SaveChangesAsync();
        }
    }
}