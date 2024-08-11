using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Repository.Store
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

    }
}
