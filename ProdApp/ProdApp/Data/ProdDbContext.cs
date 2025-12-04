using Microsoft.EntityFrameworkCore;
using ProdApp.Models;

namespace ProdApp.Data
{
    public class ProdDbContext : DbContext
    {
        public ProdDbContext(DbContextOptions<ProdDbContext> options) : base(options)
        {
                
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new ProductConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
        }
    }
}
