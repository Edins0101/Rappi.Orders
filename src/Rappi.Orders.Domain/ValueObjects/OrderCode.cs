using Rappi.Orders.Domain.Common;

namespace Rappi.Orders.Domain.ValueObjects
{
    public readonly record struct OrderCode
    {
        public string Value { get; }

        public OrderCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("OrderCode es requerido.");

            Value = value.Trim();
        }

        public override string ToString() => Value;
    }
}
