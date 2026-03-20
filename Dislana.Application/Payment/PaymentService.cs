using Dislana.Application.Order.DTOs;
using Dislana.Application.Payment.DTOs;
using Dislana.Application.Payment.Interfaces;
using Dislana.Application.Payment.Utils;
using Dislana.Domain.Payment.Interfaces;
using Dislana.Application.Secrets;
using Dislana.Application.Payment.Options;

namespace Dislana.Application.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISecretProvider _secretProvider;
        private readonly WompiOptions _wompiOptions;

        public PaymentService(IPaymentRepository paymentRepository, ISecretProvider secretProvider, WompiOptions wompiOptions)
        {
            _paymentRepository = paymentRepository;
            _secretProvider = secretProvider;
            _wompiOptions = wompiOptions ?? new WompiOptions();
        }

        public async Task ProcessWebhookAsync(WompiWebhookRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var secret = _secretProvider.GetSecret("Wompi:EventsSecret") ?? string.Empty;

            try
            {
                var valid = WompiSignatureValidator.Validate(request, secret);
                var payload = System.Text.Json.JsonSerializer.Serialize(request);

                if (valid)
                {
                    var t = request.data?.transaction;
                    if (t != null)
                    {
                        await _paymentRepository.UpdatePaymentAsync(
                            t.reference ?? string.Empty,
                            t.status ?? string.Empty,
                            t.id ?? string.Empty,
                            t.payment_method_type ?? string.Empty,
                            request.timestamp.ToString());
                    }
                }
                else
                {
                    await _paymentRepository.SavePaymentLogAsync(request.data?.transaction?.reference ?? string.Empty, payload, "Invalid signature");
                }
            }
            catch (Exception ex)
            {
                var payload = System.Text.Json.JsonSerializer.Serialize(request);
                await _paymentRepository.SavePaymentLogAsync(request.data?.transaction?.reference ?? string.Empty, payload, ex.Message);
            }
        }

        public async Task<WompiPaymentDto> CreatePaymentAsync(string login, PaymentRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var reference = PaymentUtils.CreateReference(login);
            var pedido = PaymentUtils.BuildItemsXml(request.Items);

            var opts = _wompiOptions;

            string GetConfig(string? optValue, params string[] keys)
            {
                if (!string.IsNullOrWhiteSpace(optValue))
                    return optValue!;

                foreach (var key in keys)
                {
                    var val = _secretProvider.GetSecret(key);
                    if (!string.IsNullOrWhiteSpace(val))
                        return val!;
                }

                return string.Empty;
            }

            var publicKey = GetConfig(opts?.PublicKey, "WOMPI_PUBLICKEY", "Wompi:PublicKey");
            var integritySecret = GetConfig(opts?.PrivateKey, "WOMPI_INTEGRITYSECRET", "Wompi:IntegritySecret");
            var redirectUrl = GetConfig(opts?.PublicKey, "WOMPI_REDIRECTURL", "Wompi:RedirectUrl");
            var urlBase = GetConfig(opts?.PublicKey, "WOMPI_URLBASE", "Wompi:UrlBase");

            var currency = opts?.Currency ?? "COP";

            if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(integritySecret) || string.IsNullOrWhiteSpace(redirectUrl) || string.IsNullOrWhiteSpace(urlBase))
                throw new InvalidOperationException("Wompi keys are not configured.");

            var amountInCents = (long)Math.Round(request.ValorTotal * 100m);

            string raw = reference + amountInCents + currency + integritySecret;
            string signature = PaymentUtils.CreateSign(raw);

            await _paymentRepository.SavePaymentAsync(login, reference, "PENDING", pedido, request.ValorTotal, cancellationToken);

            return new WompiPaymentDto(publicKey, currency, amountInCents, reference, signature, redirectUrl, urlBase);
        }

        public async Task<PaymentResponseDto> SaveOrderOnlyAsync(string login, PaymentRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var reference = PaymentUtils.CreateReference(login);
            var pedido = PaymentUtils.BuildItemsXml(request.Items);

            await _paymentRepository.SavePaymentAsync(login, reference, "PRINT", pedido, request.ValorTotal, cancellationToken);

            return new PaymentResponseDto(reference, request.ValorTotal);
        }
    }
}
