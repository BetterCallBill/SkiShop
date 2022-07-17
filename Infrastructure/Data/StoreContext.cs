using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContext : DbContext
    {
        // Create connection string
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }
        
        // Mapping to Product table
        public DbSet<Product> Products { get; set; }
    }
}