using Dislana.Application.ChatAssistant.Interfaces;
using Dislana.Application.ChatAssistant.Request;
using Dislana.Application.ChatAssistant.Response;
using Dislana.Application.Common.Interfaces;
using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.Exceptions;
using System.Globalization;
using System.Text;

namespace Dislana.Application.ChatAssistant.Services
{
    public  class ChatAssistantService : IChatAssistantService
    {
        private readonly IChatSessionRepository _sessionRepository;
        private readonly IOpenAIService _openAIService;
        private readonly IChatInvoiceRepository _chatInvoiceRepository;
        private readonly IUserContextService _userContextService;
        private readonly IPdfReportGenerator _pdfReportGenerator;
        private readonly IScheduledMessageRepository _scheduledMessageRepository;
        private readonly IProductChatRepository _productChatRepository;
        private readonly IPaymentChatRepository _paymentChatRepository;

        public ChatAssistantService(
            IChatSessionRepository sessionRepository,
            IOpenAIService openAIService,
            IChatInvoiceRepository chatInvoiceRepository,
            IUserContextService userContextService,
            IPdfReportGenerator pdfReportGenerator,
            IScheduledMessageRepository scheduledMessageRepository,
            IProductChatRepository productChatRepository,
            IPaymentChatRepository paymentChatRepository)
        {
            _sessionRepository = sessionRepository;
            _openAIService = openAIService;
            _chatInvoiceRepository = chatInvoiceRepository;
            _userContextService = userContextService;
            _pdfReportGenerator = pdfReportGenerator;
            _scheduledMessageRepository = scheduledMessageRepository;
            _productChatRepository = productChatRepository;
            _paymentChatRepository = paymentChatRepository;
        }

        public async Task<ChatMessageResponse> ProcessMessageAsync(ChatMessageRequest request, CancellationToken cancellationToken)
        {
            // Obtener datos del cliente
            var userIdString = _userContextService.GetId();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");
            }

            var userMessage = request.Message.Trim();

            // Detectar intención
            var normalizedMessage = userMessage.ToLowerInvariant();

            var needsProducts =
                normalizedMessage.Contains("producto") ||
                normalizedMessage.Contains("catalogo") ||
                normalizedMessage.Contains("catálogo") ||
                normalizedMessage.Contains("comprar") ||
                normalizedMessage.Contains("pedido");

            var needsPayments =
                normalizedMessage.Contains("pago") ||
                normalizedMessage.Contains("abono") ||
                normalizedMessage.Contains("consignación");

            var invoiceRecordsResult = await _chatInvoiceRepository.GetChatInvoiceByUserIdAsync(userIdString, cancellationToken);
            var invoiceRecords = invoiceRecordsResult.ToList();
            var customerData = FormatInvoiceData(invoiceRecords);
            var customerName = invoiceRecords.Count > 0 ? invoiceRecords[0].Customer.Trim() : null;

            // Cargar solo si se necesita
            Task<IEnumerable<ProductEntity>>? productTask = null;
            Task<IEnumerable<PaymentEntity>>? paymentTask = null;

            if (needsProducts)
            {
                productTask = _productChatRepository
                    .GetActiveProductsAsync(cancellationToken);
            }

            if (needsPayments)
            {
                paymentTask = _paymentChatRepository
                    .GetPaymentsByUserIdAsync(
                        userIdString,
                        cancellationToken);
            }

            if (productTask is not null)
                await productTask;

            if (paymentTask is not null)
                await paymentTask;

            var productList = productTask is not null ? productTask.Result.ToList() : new List<ProductEntity>();
            var productData = FormatProductData(productList);

            var paymentList = paymentTask is not null ? paymentTask.Result.ToList() : new List<PaymentEntity>();
            var paymentData = FormatPaymentData(paymentList);

            var session = _sessionRepository.GetSession(request.SessionId);
            if (session == null)
            {
                session = new ChatSessionEntity
                {
                    SessionId = request.SessionId,
                    UserId = userIdString,
                    History = new List<ChatMessageEntity>(),
                    WaitingForPdf = false,
                    PendingPdfType = string.Empty
                };
            }

            if (session.WaitingForPdf)
            {
                var message = userMessage.ToLower();
                var confirmWords = new[]
                {
                    "sí", "si", "yes", "claro", "ok", "dale", "listo", "quiero",
                    "mándalo", "mandalo", "envíalo", "envialo", "pdf", "reporte",
                    "informe", "resumen", "extracto", "por favor"
                };

                var rejectWords = new[]
                {
                    "no", "nope", "no gracias", "no quiero", "no necesito"
                };

                var isConfirmed = confirmWords.Any(word => message.Contains(word));
                var isRejected = rejectWords.Any(word => message.Contains(word));

                if (isConfirmed)
                {
                    session.WaitingForPdf = false;
                    _sessionRepository.SaveSession(session);

                    return new ChatMessageResponse(
                        "📄 Tu reporte está listo.",
                        false,
                        session.PendingPdfType,
                        "pdf_pendiente"
                    );
                }

                if (isRejected)
                {
                    session.WaitingForPdf = false;
                    session.PendingPdfType = string.Empty;

                    _sessionRepository.SaveSession(session);

                    return new ChatMessageResponse(
                        "¡Está bien! Si en algún momento lo necesitas, solo dímelo. 😊",
                        false,
                        null,
                        "normal"
                    );
                }

                session.WaitingForPdf = false;
            }

            // Preparar prompt para OpenAI
            var isFirstMessage = session.History.Count == 0;

            // Obtener mensajes programados activos
            var scheduledMessages = await _scheduledMessageRepository.GetActiveMessagesAsync(cancellationToken);

            var systemPrompt = BuildSystemPrompt(
                isFirstMessage,
                customerName,
                customerData,
                invoiceRecords.Count,
                scheduledMessages,
                productData,
                productList.Count,
                paymentData,
                paymentList.Count
            );

            session.History = session.History
                .TakeLast(8)
                .ToList();

            // Agregar mensaje del usuario al historial
            session.History.Add(new ChatMessageEntity
            {
                Role = "user",
                Content = userMessage
            });

            // Llamar a OpenAI
            var responseText = await _openAIService.SendMessageAsync(session, systemPrompt, cancellationToken);

            // Detectar [OFRECER_PDF]
            var offerPdf = false;

            if (responseText.Contains("[OFRECER_PDF]") && invoiceRecords.Count > 0)
            {
                offerPdf = true;
                responseText = responseText
                    .Replace("[OFRECER_PDF]", "")
                    .Trim();

                session.PendingPdfType = DetectPdfType(userMessage);
                session.WaitingForPdf = true;
            }

            // Agregar respuesta al historial
            session.History.Add(new ChatMessageEntity
            {
                Role = "assistant",
                Content = responseText
            });

            // Mantener solo los últimos 8 mensajes
            session.History = session.History
                .TakeLast(8)
                .ToList();

            if (offerPdf)
            {
                responseText += "\n\n📄 ¿Deseas que te envíe un PDF con el resumen de tu cuenta?";
            }

            _sessionRepository.SaveSession(session);

            return new ChatMessageResponse(
                responseText,
                offerPdf,
                offerPdf ? session.PendingPdfType : null,
                "texto"
            );
        }

        private static string FormatInvoiceData(List<ChatInvoiceEntity> records)
        {
            if (records.Count == 0)
                return "No se encontraron registros para este cliente.";

            var totalSales = records.Sum(r => r.Valor);
            var totalBalance = records.Sum(r => r.Saldo);

            var sb = new StringBuilder();
            sb.AppendLine("RESUMEN FINANCIERO:");
            sb.AppendLine($"- Total ventas: ${totalSales.ToString("N0", new CultureInfo("es-CO"))}");
            sb.AppendLine($"- Total saldo pendiente: ${totalBalance.ToString("N0", new CultureInfo("es-CO"))}");
            sb.AppendLine($"- Número de facturas: {records.Count}");
            sb.AppendLine();
            sb.AppendLine("DETALLE DE REGISTROS:");

            for (int i = 0; i < records.Count; i++)
            {
                var record = records[i];

                sb.AppendLine($"\nRegistro {i + 1}:");
                sb.AppendLine($"- Tipo: {record.TypeDocument}");
                sb.AppendLine($"- Número: {record.Number}");
                sb.AppendLine($"- Fecha: {record.Fecha}");
                sb.AppendLine($"- Cliente: {record.Customer}");
                sb.AppendLine($"- Cédula/NIT: {record.CustomerDni}");
                sb.AppendLine($"- Valor: ${record.Valor.ToString("N0", new CultureInfo("es-CO"))}");
                sb.AppendLine($"- Saldo: ${record.Saldo.ToString("N0", new CultureInfo("es-CO"))}");
                sb.AppendLine($"- Guía: {(string.IsNullOrEmpty(record.Guia) ? "No asignada" : record.Guia)}");
                sb.AppendLine($"- Estado factura enviada: {(record.Enviado == 1 ? "Sí" : "No")}");
                sb.AppendLine($"- Estado guía enviada: {(record.EnviadoGuia == 1 ? "Sí" : "No")}");
                sb.AppendLine($"- Link factura: {(string.IsNullOrEmpty(record.LinkInvoice) ? "No disponible" : record.LinkInvoice)}");
                sb.AppendLine($"- Link guía: {(string.IsNullOrEmpty(record.LinkGuia) ? "No disponible" : record.LinkGuia)}");
            }

            return sb.ToString();
        }

        private static string FormatProductData(List<ProductEntity> products)
        {
            if (products.Count == 0)
                return "No hay productos disponibles en este momento.";

            var sb = new StringBuilder();
            sb.AppendLine("PRODUCTOS DISPONIBLES:");
            sb.AppendLine($"- Total de productos: {products.Count}");
            sb.AppendLine();

            // Agrupar por categoría
            var groupedByCategory = products.GroupBy(p => p.Categoria);

            foreach (var categoryGroup in groupedByCategory.Take(10))
            {
                sb.AppendLine($"\nCategoría: {categoryGroup.Key}");
                foreach (var product in categoryGroup.Take(5))
                {
                    sb.AppendLine($"  - {product.Nombre} ({product.CodItem})");
                    sb.AppendLine($"    Descripción: {product.DesItem}");
                    sb.AppendLine($"    Precio: ${product.PVP.ToString("N0", new CultureInfo("es-CO"))}");
                    if (product.PVP_DCTO > 0 && product.PVP_DCTO < product.PVP)
                    {
                        sb.AppendLine($"    Precio con descuento: ${product.PVP_DCTO.ToString("N0", new CultureInfo("es-CO"))} ({product.PorDes}% OFF)");
                    }
                    sb.AppendLine($"    Disponible: {product.Disponible} unidades");
                    sb.AppendLine($"    Color: {product.Color}");
                    sb.AppendLine($"    Ubicación: {product.NomCiu}, {product.NomDep}");
                }
            }

            sb.AppendLine("\n(Mostrando un resumen de productos. Hay más disponibles en el catálogo.)");

            return sb.ToString();
        }

        private static string FormatPaymentData(List<PaymentEntity> payments)
        {
            if (payments.Count == 0)
                return "No se encontraron pagos registrados para este cliente.";

            var totalPayments = payments.Sum(p => p.Pago);

            var sb = new StringBuilder();
            sb.AppendLine("HISTORIAL DE PAGOS:");
            sb.AppendLine($"- Total pagado: ${totalPayments.ToString("N0", new CultureInfo("es-CO"))}");
            sb.AppendLine($"- Número de pagos: {payments.Count}");
            sb.AppendLine();
            sb.AppendLine("DETALLE DE PAGOS:");

            for (int i = 0; i < payments.Count; i++)
            {
                var payment = payments[i];

                sb.AppendLine($"\nPago {i + 1}:");
                sb.AppendLine($"- Tipo: {payment.Tipo}");
                sb.AppendLine($"- Número: {payment.Numero}");
                sb.AppendLine($"- Fecha: {payment.Fecha:dd/MM/yyyy}");
                sb.AppendLine($"- Monto: ${payment.Pago.ToString("N0", new CultureInfo("es-CO"))}");
                sb.AppendLine($"- Referencia: {(string.IsNullOrEmpty(payment.Referencia) ? "No disponible" : payment.Referencia)}");
            }

            return sb.ToString();
        }

        private static string BuildSystemPrompt(
            bool isFirstMessage,
            string? customerName,
            string customerData,
            int recordsCount,
            IEnumerable<ScheduledMessageEntity> scheduledMessages,
            string productData,
            int productCount,
            string paymentData,
            int paymentCount)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Eres un asistente virtual de Textiles Dislana.");
            sb.AppendLine("Respondes preguntas de clientes sobre facturas, saldos, pedidos, guías de envío, productos y pagos.");
            sb.AppendLine("Sé amable, claro y conciso. Responde siempre en español.");
            sb.AppendLine("No inventes información — usa solo los datos que se te proporcionan.");

            if (isFirstMessage && !string.IsNullOrEmpty(customerName))
            {
                sb.AppendLine($"Es el primer mensaje del cliente. Salúdalo por su nombre: \"{customerName}\" y dale la bienvenida a Textiles Dislana.");

                // Agregar mensajes programados al saludo
                var activeMessages = scheduledMessages.Where(m => m.IsActive()).ToList();
                if (activeMessages.Any())
                {
                    sb.AppendLine("IMPORTANTE: Después del saludo, agrega la siguiente información:");
                    foreach (var message in activeMessages)
                    {
                        sb.AppendLine($"- {message.Message}");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(customerName))
            {
                sb.AppendLine($"El nombre del cliente es: {customerName}.");
            }
            else
            {
                sb.AppendLine("El cliente no tiene registros. Saluda con una bienvenida genérica a Textiles Dislana.");
            }

            sb.AppendLine("Si te preguntan por fecha de pago, toma la fecha de la factura y súmale 30 días. e invitalo a pagar en linea en el menu cotizar");
            sb.AppendLine("También puedes informar el saldo de la factura.");

            sb.AppendLine("Si el cliente quiere comprar o buscar productos, indícale que puede hacerlo desde nuestro catalogo o tienda en línea, tambien invitalo a cotizar y pagar en linea en el menu cotizar");
            sb.AppendLine("https://www.uniline.com.co/");

            sb.AppendLine("Si desean realizar un nuevo pedido, comparte este enlace:");
            sb.AppendLine("https://ecommerce.dislana.com/dist/Dislana/#/");

            sb.AppendLine("Si preguntan por horarios, informa:");
            sb.AppendLine("Lunes a viernes de 5:00 a.m. a 4:00 p.m.");
            sb.AppendLine("Dirección: Calle 9 #41A-16 Bogotá.");

            sb.AppendLine("Si el cliente pregunta algo que no está en los datos proporcionados, responde que no tienes esa información.");

            sb.AppendLine("IMPORTANTE:");
            sb.AppendLine("- Solo ofrece descargar o enviar el PDF en la primera respuesta relacionada con facturas o saldos.");
            sb.AppendLine("- Después de ofrecerlo una vez, no vuelvas a ofrecer el PDF automáticamente.");
            sb.AppendLine("- Si el cliente escribe palabras como: 'envíame', 'enviame', 'pdf', 'descargar factura' o similares, entonces sí ofrece nuevamente el PDF.");
            sb.AppendLine("- Cuando debas ofrecer el PDF, agrega exactamente al final del mensaje: [OFRECER_PDF]");

            sb.AppendLine();
            sb.AppendLine("Datos actuales del cliente:");
            sb.AppendLine(customerData);

            if (productCount > 0)
            {
                sb.AppendLine();
                sb.AppendLine("CATÁLOGO DE PRODUCTOS DISPONIBLES:");
                sb.AppendLine("Cuando el cliente pregunte por productos, usa esta información para responder:");
                sb.AppendLine(productData);
                sb.AppendLine();
                sb.AppendLine("Para más detalles de productos o realizar un pedido, siempre dirige al cliente a:");
                sb.AppendLine("https://www.uniline.com.co/ o https://ecommerce.dislana.com/dist/Dislana/#/");
            }

            if (paymentCount > 0)
            {
                sb.AppendLine();
                sb.AppendLine("HISTORIAL DE PAGOS DEL CLIENTE:");
                sb.AppendLine("Cuando el cliente pregunte por pagos realizados, usa esta información:");
                sb.AppendLine(paymentData);
            }

            return sb.ToString();
        }

        private static string DetectPdfType(string message)
        {
            var normalizedMessage = message.ToLower();

            if (
                normalizedMessage.Contains("saldo") ||
                normalizedMessage.Contains("debo") ||
                normalizedMessage.Contains("deuda") ||
                normalizedMessage.Contains("pendiente")
            )
            {
                return "saldo";
            }

            if (
                normalizedMessage.Contains("venta") ||
                normalizedMessage.Contains("compra") ||
                normalizedMessage.Contains("factura") ||
                normalizedMessage.Contains("pedido")
            )
            {
                return "ventas";
            }

            return "completo";
        }

        public async Task<GeneratePdfReportResponse> GeneratePdfReportAsync(
            GeneratePdfReportRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userIdString = _userContextService.GetId();
                if (string.IsNullOrEmpty(userIdString))
                    return GeneratePdfReportResponse.Fail("Usuario no autenticado");

                var invoicesResult = await _chatInvoiceRepository.GetChatInvoiceByUserIdAsync(
                    userIdString, 
                    cancellationToken);

                var invoices = invoicesResult.ToList();

                if (invoices.Count == 0)
                    return GeneratePdfReportResponse.Fail("No hay facturas disponibles para generar el reporte");

                var customerName = invoices.FirstOrDefault()?.Customer?.Trim() 
                    ?? "No identificado";

                var report = InvoiceReportEntity.Create(
                    request.Tipo,
                    customerName,
                    invoices
                );

                report.ValidateForGeneration();

                var pdfBytes = await _pdfReportGenerator.GeneratePdfAsync(report, cancellationToken);
                var fileName = report.GenerateFileName();

                return GeneratePdfReportResponse.Success(pdfBytes, fileName);
            }
            catch (DomainException ex)
            {
                return GeneratePdfReportResponse.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return GeneratePdfReportResponse.Fail($"Error al generar PDF: {ex.Message}");
            }
        }
    }
}

