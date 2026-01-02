using Rappi.Orders.Domain.Common;
using Rappi.Orders.Domain.Entities;
using Rappi.Orders.Domain.Enums;
using Rappi.Orders.Domain.ValueObjects;

namespace Rappi.Orders.Domain.Aggregates
{
    public sealed class OrderAggregate
    {
        public AggregatorOrderId AggregatorOrder { get; }
        private readonly List<OrderItem> _items;

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public OrderAggregate(AggregatorOrderId aggregatorOrder, IEnumerable<OrderItem> items)
        {
            AggregatorOrder = aggregatorOrder;
            _items = items?.ToList() ?? throw new DomainException("Items requeridos.");

            if (_items.Count == 0)
                throw new DomainException("El pedido no puede estar vacío.");

            if (_items.Any(i => i.AggregatorOrder != aggregatorOrder))
                throw new DomainException("Todos los items deben pertenecer al mismo AggregatorOrder.");

            EnsureConsistentStatus();
        }

        public OrderStatus CurrentStatus => _items[0].Status;

        public Money Total()
        {
            var total = Money.Zero;
            foreach (var i in _items)
                total += i.Value;

            return total; // ya viene redondeado por Money
        }

        public void ChangeStatus(OrderStatus newStatus, string changedBy, DateTimeOffset changedAt)
        {
            EnsureConsistentStatus();

            var current = CurrentStatus;

            // Regla 1: Shipped no puede cancelarse
            if (current == OrderStatus.Shipped && newStatus == OrderStatus.Cancelled)
                throw new DomainException("Un pedido Shipped no puede ser cancelado.");

            // Regla 2: Paid solo puede pasar a Shipped
            if (current == OrderStatus.Paid && newStatus != OrderStatus.Shipped)
                throw new DomainException("Un pedido en estado Paid solo puede pasar a Shipped.");

            // Opcional: no permitir cambios desde Cancelled
            if (current == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
                throw new DomainException("Un pedido Cancelled no puede cambiar de estado.");

            // Aplica a todos los items
            foreach (var item in _items)
                item.SetStatus(newStatus, changedBy, changedAt);
        }

        private void EnsureConsistentStatus()
        {
            var status = _items[0].Status;
            if (_items.Any(i => i.Status != status))
                throw new DomainException("Estado inconsistente: los items del mismo AggregatorOrder deben compartir el mismo estado.");
        }
    }
}
