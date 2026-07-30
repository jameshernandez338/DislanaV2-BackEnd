namespace Dislana.Domain.Quote.Entities
{
    public class CustomerAddressEntity
    {
        public string Code { get; private set; }
        public string City { get; private set; }
        public string Address { get; private set; }

        private CustomerAddressEntity(
            string code ,
            string city,
            string address)
        {
            Code = code;
            City = city;
            Address = address;
        }

        public static CustomerAddressEntity Create(
            string code,
            string city,
            string address)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty.", nameof(code));

            return new CustomerAddressEntity(
                code,
                city,
                address);
        }
    }
}
