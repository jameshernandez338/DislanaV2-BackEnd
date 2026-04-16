namespace Dislana.Domain.Order.ValueObjects
{
    /// <summary>
    /// Value Object: Representa una cantidad con validaciones de dominio
    /// </summary>
    public sealed record Quantity
    {
        public decimal Value { get; }

        private Quantity(decimal value)
        {
            Value = value;
        }

        public static Quantity Create(decimal value)
        {
            if (value < 0)
                throw new Exceptions.DomainException("La cantidad no puede ser negativa");

            return new Quantity(value);
        }

        public static Quantity Zero => new(0);

        public static implicit operator decimal(Quantity quantity) => quantity.Value;
    }
}
