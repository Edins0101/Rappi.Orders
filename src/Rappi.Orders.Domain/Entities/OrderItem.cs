using Rappi.Orders.Domain.Common;
using Rappi.Orders.Domain.Enums;
using Rappi.Orders.Domain.ValueObjects;

namespace Rappi.Orders.Domain.Entities
{
    public sealed class OrderItem : Entity<Guid>
    {
        public OrderCode OrderCode { get; private set; }
        public AggregatorOrderId AggregatorOrder { get; private set; }
        public string Description { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public OrderStatus Status { get; private set; }
        public Money Value { get; private set; }

        // Auditoría (en dominio como info, pero quien la setea será Application/Infra)
        public string CreatedBy { get; private set; }
        public string? UpdatedBy { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public DateTimeOffset? CancelledAt { get; private set; }

        private OrderItem() { } // EF Core

        public OrderItem(
            Guid id,
            OrderCode orderCode,
            AggregatorOrderId aggregatorOrder,
            string description,
            DateTimeOffset createdAt,
            OrderStatus status,
            Money value,
            string createdBy)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description es requerida.");
            if (string.IsNullOrWhiteSpace(createdBy))
                throw new DomainException("CreatedBy es requerido.");

            Id = id;
            OrderCode = orderCode;
            AggregatorOrder = aggregatorOrder;
            Description = description.Trim();
            CreatedAt = createdAt;
            Status = status;
            Value = value;
            CreatedBy = createdBy.Trim();
        }

        internal void SetStatus(OrderStatus newStatus, string changedBy, DateTimeOffset changedAt)
        {
            Status = newStatus;
            UpdatedBy = changedBy;
            UpdatedAt = changedAt;

            if (newStatus == OrderStatus.Cancelled)
                CancelledAt = changedAt;
        }
    }
}
