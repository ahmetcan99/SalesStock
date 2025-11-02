using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesStock.Domain.Entities;
using SalesStock.Infrastructure.Identity;

namespace SalesStock.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();    
        public DbSet<Product> Products => Set<Product>();
        public DbSet<PriceList> PriceLists => Set<PriceList>();
        public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Customer>().HasIndex(x => x.Code).IsUnique();

            builder.Entity<Product>().HasIndex(x => x.SKU).IsUnique();
            builder.Entity<Product>().Property(x => x.VatRate).HasPrecision(5, 4);
            builder.Entity<Product>().Property(x => x.UnitPrice).HasPrecision(18, 2);
            builder.Entity<Product>().Property(x => x.StockOnHand).HasPrecision(18, 3);
            builder.Entity<Product>().Property(x => x.StockReserved).HasPrecision(18, 3);

            builder.Entity<PriceListItem>().Property(x => x.UnitPrice).HasPrecision(18, 2);

            builder.Entity<StockMovement>().Property(x => x.Quantity).HasPrecision(18, 3);

            builder.Entity<OrderItem>().Property(x => x.Quantity).HasPrecision(18, 3);
            builder.Entity<OrderItem>().Property(x => x.UnitPrice).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(x => x.NetTotal).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(x => x.VatTotal).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(x => x.GrandTotal).HasPrecision(18, 2);

            builder.Entity<Order>().HasIndex(x => x.OrderNo).IsUnique();
            builder.Entity<Order>().Property(x => x.NetTotal).HasPrecision(18, 2);
            builder.Entity<Order>().Property(x => x.VatTotal).HasPrecision(18, 2);
            builder.Entity<Order>().Property(x => x.GrandTotal).HasPrecision(18, 2);

            builder.Entity<PriceListItem>().HasIndex(x => new { x.PriceListId, x.ProductId, x.ValidFrom, x.ValidTo });

            builder.Entity<Customer>().Property(x => x.isActive).HasDefaultValue(true);
            builder.Entity<Product>().Property(x => x.isActive).HasDefaultValue(true);
            builder.Entity<PriceList>().Property(x => x.isActive).HasDefaultValue(true);
        }

    }
}