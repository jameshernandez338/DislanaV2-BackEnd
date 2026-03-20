namespace Dislana.Application.Payment.DTOs
{
    public class WompiWebhookRequest
    {
        public string? @event { get; set; }
        public WompiData? data { get; set; }
        public string? environment { get; set; }
        public WompiSignature? signature { get; set; }
        public long timestamp { get; set; }
        public string? sent_at { get; set; }
    }

    public class WompiData
    {
        public WompiTransaction? transaction { get; set; }
    }

    public class WompiTransaction
    {
        public string? id { get; set; }
        public long amount_in_cents { get; set; }
        public string? reference { get; set; }
        public string? customer_email { get; set; }
        public string? currency { get; set; }
        public string? payment_method_type { get; set; }
        public string? redirect_url { get; set; }
        public string? status { get; set; }
        public object? shipping_address { get; set; }
        public object? payment_link_id { get; set; }
        public object? payment_source_id { get; set; }
    }

    public class WompiSignature
    {
        public List<string>? properties { get; set; }
        public string? checksum { get; set; }
    }
}
