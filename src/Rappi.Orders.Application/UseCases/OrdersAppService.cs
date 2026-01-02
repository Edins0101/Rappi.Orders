using Rappi.Orders.Application.Common.Responses;
using Rappi.Orders.Application.Dtos;
using Rappi.Orders.Application.Interfaces;
using Rappi.Orders.Domain.Aggregates;
using Rappi.Orders.Domain.Common;
using Rappi.Orders.Domain.Entities;
using Rappi.Orders.Domain.Enums;
using Rappi.Orders.Domain.ValueObjects;

namespace Rappi.Orders.Application.UseCases
{
    public sealed class OrdersAppService : IOrdersAppService
    {
        private readonly IOrdersRepository _repo;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUserService _currentUser;

        public OrdersAppService(IOrdersRepository repo, IDateTimeProvider clock, ICurrentUserService currentUser)
        {
            _repo = repo;
            _clock = clock;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<OrderTotalResponse>> CreateAsync(CreateOrderRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.AggregatorOrder))
                return ApiResponse<OrderTotalResponse>.ValidationError("AggregatorOrder es requerido.");

            if (request.Items is null || request.Items.Count == 0)
                return ApiResponse<OrderTotalResponse>.ValidationError("El pedido debe tener al menos un item.");

            // Validación rápida: OrderCode únicos en request
            var dup = request.Items
                .GroupBy(i => i.OrderCode?.Trim(), StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1);

            if (dup is not null)
                return ApiResponse<OrderTotalResponse>.ValidationError($"OrderCode duplicado en request: {dup.Key}");

            try
            {
                var agg = new AggregatorOrderId(request.AggregatorOrder);
                var now = _clock.UtcNow;
                var createdBy = _currentUser.UserName;

                var items = request.Items.Select(i =>
                    new OrderItem(
                        id: Guid.NewGuid(),
                        orderCode: new OrderCode(i.OrderCode),
                        aggregatorOrder: agg,
                        description: i.Description,
                        createdAt: now,
                        status: OrderStatus.Created,
                        value: new Money(i.Value),
                        createdBy: createdBy
                    )
                ).ToList();

                // Dominio calcula total
                var aggregate = new OrderAggregate(agg, items);
                var total = aggregate.Total().Amount;

                await _repo.AddRangeAsync(items, ct);
                await _repo.SaveChangesAsync(ct);

                return ApiResponse<OrderTotalResponse>.Success(
                    new OrderTotalResponse(agg.Value, total),
                    "Pedido creado correctamente."
                );
            }
            catch (DomainException ex)
            {
                return ApiResponse<OrderTotalResponse>.ValidationError(ex.Message);
            }
        }

        public async Task<ApiResponse<OrderTotalResponse>> GetTotalAsync(string aggregatorOrder, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(aggregatorOrder))
                return ApiResponse<OrderTotalResponse>.ValidationError("AggregatorOrder es requerido.");

            try
            {
                var agg = new AggregatorOrderId(aggregatorOrder);
                var items = await _repo.GetByAggregatorAsync(agg, ct);

                if (items.Count == 0)
                    return ApiResponse<OrderTotalResponse>.ValidationError("No existe un pedido con ese AggregatorOrder.");

                var aggregate = new OrderAggregate(agg, items);
                var total = aggregate.Total().Amount;

                return ApiResponse<OrderTotalResponse>.Success(
                    new OrderTotalResponse(agg.Value, total)
                );
            }
            catch (DomainException ex)
            {
                return ApiResponse<OrderTotalResponse>.ValidationError(ex.Message);
            }
        }

        public async Task<ApiResponse<object>> ChangeStatusAsync(ChangeStatusRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.AggregatorOrder))
                return ApiResponse<object>.ValidationError("AggregatorOrder es requerido.");

            if (string.IsNullOrWhiteSpace(request.NewStatus))
                return ApiResponse<object>.ValidationError("NewStatus es requerido.");

            if (!Enum.TryParse<OrderStatus>(request.NewStatus, ignoreCase: true, out var newStatus))
                return ApiResponse<object>.ValidationError("NewStatus inválido. Use: Created, Paid, Cancelled, Shipped.");

            try
            {
                var agg = new AggregatorOrderId(request.AggregatorOrder);
                var items = await _repo.GetByAggregatorAsync(agg, ct);

                if (items.Count == 0)
                    return ApiResponse<object>.ValidationError("No existe un pedido con ese AggregatorOrder.");

                var aggregate = new OrderAggregate(agg, items);

                aggregate.ChangeStatus(newStatus, _currentUser.UserName, _clock.UtcNow);

                await _repo.SaveChangesAsync(ct);

                return ApiResponse<object>.Success(
                    new { AggregatorOrder = agg.Value, Status = newStatus.ToString() },
                    "Estado actualizado correctamente."
                );
            }
            catch (DomainException ex)
            {
                return ApiResponse<object>.ValidationError(ex.Message);
            }
        }
    }
}
