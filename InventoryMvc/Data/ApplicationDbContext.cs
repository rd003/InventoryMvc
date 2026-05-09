using InventoryMvc.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryMvc.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>().HasQueryFilter(x => x.DeleteDate == null);
            builder.Entity<Product>().HasQueryFilter(x => x.DeleteDate == null);
            builder.Entity<Supplier>().HasQueryFilter(x => x.DeleteDate == null);
            builder.Entity<Purchase>().HasQueryFilter(x => x.DeleteDate == null);
            builder.Entity<Sale>().HasQueryFilter(x => x.DeleteDate == null);
            builder.Entity<Stock>().HasQueryFilter(x => x.DeleteDate == null);
            base.OnModelCreating(builder);
        }
    }
}
