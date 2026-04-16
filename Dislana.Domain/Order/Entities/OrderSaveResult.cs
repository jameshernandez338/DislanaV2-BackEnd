namespace Dislana.Domain.Order.Results
{
    /// <summary>
    /// Result Object: Representa el resultado de guardar una orden
    /// </summary>
    public sealed record OrderSaveResult
    {
        public string Message { get; }
        public bool IsSuccess => !string.IsNullOrWhiteSpace(Message);

        private OrderSaveResult(string message)
        {
            Message = message ?? string.Empty;
        }

        public static OrderSaveResult Success(string message) => new(message);

        public static OrderSaveResult Failure(string error) => new(error);
    }
}
