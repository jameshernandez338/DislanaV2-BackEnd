using System.Xml.Linq;

namespace Dislana.Domain.Order.Entities
{
    /// <summary>
    /// Aggregate Root: Orden con toda la lógica de negocio centralizada
    /// </summary>
    public class OrderEntity
    {
        public string Id { get; private set; }
        public string CreatedBy { get; private set; }
        public string Observation { get; private set; }
        public DateTime CreatedAt { get; private set; }
        
        private readonly List<OrderItemEntity> _items = new();
        public IReadOnlyList<OrderItemEntity> Items => _items.AsReadOnly();

        // Constructor privado para encapsulación
        private OrderEntity(string createdBy, string observation)
        {
            Id = Guid.NewGuid().ToString();
            CreatedBy = createdBy;
            Observation = observation;
            CreatedAt = DateTime.UtcNow;
        }

        // Factory Method: punto de entrada con validaciones
        public static OrderEntity Create(string userName, string? observation = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new Exceptions.DomainException("El usuario es requerido para crear una orden");

            return new OrderEntity(userName.Trim(), observation?.Trim() ?? string.Empty);
        }

        // Comportamiento: agregar item con validaciones de negocio
        public void AddItem(OrderItemEntity item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // Regla de negocio: máximo 100 items por orden
            if (_items.Count >= 100)
                throw new Exceptions.DomainException("No se pueden agregar más de 100 items a una orden");

            // Regla de negocio: no duplicar productos
            if (_items.Any(i => i.ProductCode.Value == item.ProductCode.Value))
                throw new Exceptions.DomainException($"El producto '{item.ProductCode}' ya existe en la orden");

            _items.Add(item);
        }

        // Validación de negocio: orden debe tener items
        public void ValidateForSave()
        {
            if (!_items.Any())
                throw new Exceptions.DomainException("La orden debe tener al menos un item");
        }

        // Comportamiento: calcular total de la orden
        public decimal CalculateTotal()
        {
            return _items.Sum(item => item.CalculateSubtotal().Amount);
        }

        // Comportamiento: serialización a XML (formato legacy para SP)
        public string ToXml()
        {
            var root = new XElement("Items", _items.Select(i => i.ToXmlElement()));
            return new XDocument(root).ToString(SaveOptions.DisableFormatting);
        }

        // Constructor sin parámetros para ORM
        private OrderEntity()
        {
            Id = string.Empty;
            CreatedBy = string.Empty;
            Observation = string.Empty;
        }
    }
}
