namespace Dislana.Domain.Auth.ValueObjects
{
    /// <summary>
    /// Value Object: Representa un password ya hasheado
    /// </summary>
    public sealed record HashedPassword
    {
        public string Value { get; }

        private HashedPassword(string value)
        {
            Value = value;
        }

        public static HashedPassword Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exceptions.DomainException("El password hash es requerido");

            // BCrypt hashes tienen longitud específica (~60 caracteres)
            if (value.Length < 20)
                throw new Exceptions.DomainException("Password hash inválido");

            return new HashedPassword(value);
        }

        public static implicit operator string(HashedPassword password) => password.Value;

        public override string ToString() => "[PROTECTED]"; // Por seguridad, no exponemos el hash
    }
}
