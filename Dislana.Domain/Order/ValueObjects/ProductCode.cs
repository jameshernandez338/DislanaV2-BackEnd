namespace Dislana.Domain.Order.ValueObjects
{
    /// <summary>
    /// Value Object: Representa el código de un producto con validaciones
    /// </summary>
    public sealed record ProductCode
    {
        public string Value { get; }

        private ProductCode(string value)
        {
            Value = value;
        }

        public static ProductCode Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exceptions.DomainException("El código del producto es requerido");

            return new ProductCode(value.Trim());
        }

        public static implicit operator string(ProductCode code) => code.Value;

        public override string ToString() => Value;
    }
}
