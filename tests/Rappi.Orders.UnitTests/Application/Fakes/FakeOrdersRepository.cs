using Rappi.Orders.Application.Interfaces;
using Rappi.Orders.Domain.Entities;
using Rappi.Orders.Domain.Enums;
using Rappi.Orders.Domain.ValueObjects;

namespace Rappi.Orders.UnitTests.Application.Fakes
{
    public sealed class FakeOrdersRepository : IOrdersRepository
    {
        private readonly List<OrderItem> _items = new();

        public Task AddRangeAsync(IEnumerable<OrderItem> items, CancellationToken ct)
        {
            _items.AddRange(items);
            return Task.CompletedTask;
        }

        public Task<List<OrderItem>> GetByAggregatorAsync(AggregatorOrderId aggregatorOrder, CancellationToken ct)
            => Task.FromResult(_items.Where(x => x.AggregatorOrder == aggregatorOrder).ToList());

        public Task<OrderItem?> GetByOrderCodeAsync(string orderCode, CancellationToken ct)
            => Task.FromResult(_items.FirstOrDefault(x => x.OrderCode.Value == orderCode));

        public Task<int> SaveChangesAsync(CancellationToken ct) => Task.FromResult(1);

        public Task<List<OrderItem>> GetCreatedOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct)
            => Task.FromResult(_items.Where(x => x.Status == OrderStatus.Created && x.CreatedAt < cutoff).ToList());
    }
}
