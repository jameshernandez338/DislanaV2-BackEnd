using Dislana.Domain.Order.ValueObjects;
using System.Globalization;
using System.Xml.Linq;

namespace Dislana.Domain.Order.Entities
{
    /// <summary>
    /// Entidad Rica: Acabado de tela con comportamiento y reglas de negocio
    /// </summary>
    public class FabricFinishEntity
    {
        public string Name { get; private set; }
        public bool RequiresText { get; private set; }
        public string Text { get; private set; }
        public Money Price { get; private set; }

        // Constructor privado para encapsulación
        private FabricFinishEntity(string name, bool requiresText, string text, Money price)
        {
            Name = name;
            RequiresText = requiresText;
            Text = text;
            Price = price;
        }

        // Factory Method con validaciones de dominio
        public static FabricFinishEntity Create(string name, bool requiresText, string? text, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exceptions.DomainException("El nombre del acabado es requerido");

            if (requiresText && string.IsNullOrWhiteSpace(text))
                throw new Exceptions.DomainException("El texto es requerido cuando el acabado lo requiere");

            return new FabricFinishEntity(
                name.Trim(), 
                requiresText, 
                text?.Trim() ?? string.Empty, 
                Money.Create(price)
            );
        }

        // Comportamiento: serialización a XML (conocimiento del dominio)
        public XElement ToXmlElement()
        {
            return new XElement("Acabado",
                new XElement("Nombre", Name),
                new XElement("Texto", Text),
                new XElement("Valor", Price.Amount.ToString(CultureInfo.InvariantCulture))
            );
        }

        // Constructor sin parámetros para Dapper
        private FabricFinishEntity()
        {
            Name = string.Empty;
            Text = string.Empty;
            Price = Money.Zero;
        }

        // Propiedades para mapeo de Dapper (desde SP)
        public string Acabado 
        { 
            get => Name;
            set => Name = value; 
        }

        public bool TieneTexto 
        { 
            get => RequiresText;
            set => RequiresText = value; 
        }

        public decimal Valor 
        { 
            get => Price.Amount;
            set => Price = Money.Create(value); 
        }
    }
}
