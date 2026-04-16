namespace Dislana.Domain.Order.ValueObjects
{
    /// <summary>
    /// Value Object: Representa dinero con validaciones de dominio
    /// </summary>
    public sealed record Money
    {
        public decimal Amount { get; }

        private Money(decimal amount)
        {
            Amount = amount;
        }

        public static Money Create(decimal amount)
        {
            if (amount < 0)
                throw new Exceptions.DomainException("El monto no puede ser negativo");

            return new Money(amount);
        }

        public static Money Zero => new(0);

        public Money Add(Money other) => new(Amount + other.Amount);

        public Money Multiply(decimal factor) => new(Amount * factor);

        public static implicit operator decimal(Money money) => money.Amount;
    }
}
