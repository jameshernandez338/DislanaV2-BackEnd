namespace Dislana.Domain.Order.Entities
{
    public class OrderSaveResult
    {
        public string Message { get; private set; } = string.Empty;

        private OrderSaveResult() { }

        public OrderSaveResult(string message)
        {
            Message = message ?? string.Empty;
        }
    }
}
