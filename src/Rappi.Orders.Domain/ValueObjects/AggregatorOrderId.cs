using Rappi.Orders.Domain.Common;

namespace Rappi.Orders.Domain.ValueObjects
{
    public readonly record struct AggregatorOrderId
    {
        public string Value { get; }

        public AggregatorOrderId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("AggregatorOrder es requerido.");

            Value = value.Trim();
        }

        public override string ToString() => Value;
    }
}
