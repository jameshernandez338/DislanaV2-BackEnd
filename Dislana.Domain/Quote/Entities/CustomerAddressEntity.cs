namespace Dislana.Domain.Quote.Entities
{
    public class CustomerAddressEntity
    {
        public string Codigo { get; private set; }
        public string Ciudad { get; private set; }
        public string Direccion { get; private set; }

        private CustomerAddressEntity(
            string codigo,
            string ciudad,
            string direccion)
        {
            Codigo = codigo;
            Ciudad = ciudad;
            Direccion = direccion;
        }

        public static CustomerAddressEntity Create(
            string codigo,
            string ciudad,
            string direccion)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Codigo cannot be empty.", nameof(codigo));

            return new CustomerAddressEntity(
                codigo,
                ciudad,
                direccion);
        }
    }
}
