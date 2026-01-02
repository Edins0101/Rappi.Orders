using Microsoft.EntityFrameworkCore;
using Rappi.Orders.Application.Interfaces;
using Rappi.Orders.Domain.Entities;
using Rappi.Orders.Domain.Enums;
using Rappi.Orders.Domain.ValueObjects;
using Rappi.Orders.Infrastructure.Persistence;

namespace Rappi.Orders.Infrastructure.Repository
{
    public sealed class OrdersRepository : IOrdersRepository
    {
        private readonly OrdersDbContext _db;

        public OrdersRepository(OrdersDbContext db) => _db = db;

        public Task AddRangeAsync(IEnumerable<OrderItem> items, CancellationToken ct)
            => _db.Orders.AddRangeAsync(items, ct);

        public Task<List<OrderItem>> GetByAggregatorAsync(AggregatorOrderId aggregatorOrder, CancellationToken ct)
            => _db.Orders
                .Where(o => o.AggregatorOrder == aggregatorOrder)
                .ToListAsync(ct);

        public Task<OrderItem?> GetByOrderCodeAsync(string orderCode, CancellationToken ct)
            => _db.Orders
                .FirstOrDefaultAsync(o => o.OrderCode.Value == orderCode, ct);

        public Task<int> SaveChangesAsync(CancellationToken ct)
            => _db.SaveChangesAsync(ct);

        public Task<List<OrderItem>> GetCreatedOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct)
            => _db.Orders
                .Where(o => o.Status == OrderStatus.Created && o.CreatedAt < cutoff)
                .ToListAsync(ct);
    }
}
