using Rappi.Orders.Domain.Common;

namespace Rappi.Orders.Domain.ValueObjects
{
    public readonly record struct Money
    {
        public decimal Amount { get; }

        public Money(decimal amount)
        {
            if (amount < 0)
                throw new DomainException("El valor no puede ser negativo.");

            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        }

        public static Money Zero => new(0m);

        public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);

        public override string ToString() => Amount.ToString("0.00");
    }
}
