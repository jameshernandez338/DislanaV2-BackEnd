using Dislana.Domain.Order.ValueObjects;
using System.Globalization;
using System.Xml.Linq;

namespace Dislana.Domain.Order.Entities
{
    /// <summary>
    /// Entidad Rica: Representa un item de orden con lógica de negocio
    /// </summary>
    public class OrderItemEntity
    {
        public ProductCode ProductCode { get; private set; }
        public Quantity Quantity1 { get; private set; }
        public Quantity QuantityB { get; private set; }
        public Money Price { get; private set; }
        public Money PriceB { get; private set; }
        
        private readonly List<FabricFinishEntity> _finishes = new();
        public IReadOnlyList<FabricFinishEntity> Finishes => _finishes.AsReadOnly();

        // Constructor privado para encapsulación
        private OrderItemEntity(
            ProductCode productCode,
            Quantity quantity1,
            Quantity quantityB,
            Money price,
            Money priceB)
        {
            ProductCode = productCode;
            Quantity1 = quantity1;
            QuantityB = quantityB;
            Price = price;
            PriceB = priceB;
        }

        // Factory Method con validaciones
        public static OrderItemEntity Create(
            string productCode,
            decimal quantity1,
            decimal quantityB,
            decimal price,
            decimal priceB)
        {
            return new OrderItemEntity(
                ValueObjects.ProductCode.Create(productCode),
                Quantity.Create(quantity1),
                Quantity.Create(quantityB),
                Money.Create(price),
                Money.Create(priceB)
            );
        }

        // Comportamiento: agregar acabado con validaciones
        public void AddFinish(FabricFinishEntity finish)
        {
            if (finish == null)
                throw new ArgumentNullException(nameof(finish));

            // Regla de negocio: no duplicar acabados
            if (_finishes.Any(f => f.Name == finish.Name))
                throw new Exceptions.DomainException($"El acabado '{finish.Name}' ya existe en este item");

            _finishes.Add(finish);
        }

        // Comportamiento: calcular subtotal del item
        public Money CalculateSubtotal()
        {
            var itemTotal = Price.Multiply(Quantity1.Value).Amount + 
                           PriceB.Multiply(QuantityB.Value).Amount;
            
            var finishesTotal = _finishes.Sum(f => f.Price.Amount);
            
            return Money.Create(itemTotal + finishesTotal);
        }

        // Comportamiento: serialización a XML
        public XElement ToXmlElement()
        {
            return new XElement("Item",
                new XElement("CodigoItem", ProductCode.Value),
                new XElement("Cantidad1", Quantity1.Value.ToString(CultureInfo.InvariantCulture)),
                new XElement("CantidadB", QuantityB.Value.ToString(CultureInfo.InvariantCulture)),
                new XElement("Pvp", Price.Amount.ToString(CultureInfo.InvariantCulture)),
                new XElement("PvpB", PriceB.Amount.ToString(CultureInfo.InvariantCulture)),
                new XElement("Acabados", _finishes.Select(f => f.ToXmlElement()))
            );
        }

        // Constructor sin parámetros para ORM/Dapper
        private OrderItemEntity()
        {
            ProductCode = ValueObjects.ProductCode.Create("TEMP");
            Quantity1 = Quantity.Zero;
            QuantityB = Quantity.Zero;
            Price = Money.Zero;
            PriceB = Money.Zero;
        }
    }
}
