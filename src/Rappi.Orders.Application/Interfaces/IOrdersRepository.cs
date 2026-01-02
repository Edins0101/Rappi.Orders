using Rappi.Orders.Domain.Entities;
using Rappi.Orders.Domain.ValueObjects;

namespace Rappi.Orders.Application.Interfaces
{
    public interface IOrdersRepository
    {
        Task AddRangeAsync(IEnumerable<OrderItem> items, CancellationToken ct);
        Task<List<OrderItem>> GetByAggregatorAsync(AggregatorOrderId aggregatorOrder, CancellationToken ct);
        Task<OrderItem?> GetByOrderCodeAsync(string orderCode, CancellationToken ct);

        Task<int> SaveChangesAsync(CancellationToken ct);

        // Para el worker
        Task<List<OrderItem>> GetCreatedOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct);
    }
}
