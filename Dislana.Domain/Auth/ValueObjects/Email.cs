using System.Text.RegularExpressions;

namespace Dislana.Domain.Auth.ValueObjects
{
    /// <summary>
    /// Value Object: Representa un email válido
    /// </summary>
    public sealed record Email
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exceptions.DomainException("El email es requerido");

            var normalizedEmail = value.Trim().ToLowerInvariant();

            if (normalizedEmail.Length > 150)
                throw new Exceptions.DomainException("El email no puede exceder 150 caracteres");

            if (!EmailRegex.IsMatch(normalizedEmail))
                throw new Exceptions.DomainException("Formato de email inválido");

            return new Email(normalizedEmail);
        }

        public static implicit operator string(Email email) => email.Value;

        public override string ToString() => Value;
    }
}
