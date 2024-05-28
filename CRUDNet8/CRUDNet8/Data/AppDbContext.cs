using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;

namespace CRUDNet8.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Product>  Products { get; set; }
    }
}
