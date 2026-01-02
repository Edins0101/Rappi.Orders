using Rappi.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Rappi.Orders.Infrastructure.Persistence
{
    public sealed class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

        public DbSet<OrderItem> Orders => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
