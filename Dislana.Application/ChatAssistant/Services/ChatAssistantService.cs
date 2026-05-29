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
        private readonly IMensajeProgramadoRepository _mensajeProgramadoRepository;

        public ChatAssistantService(
            IChatSessionRepository sessionRepository,
            IOpenAIService openAIService,
            IChatInvoiceRepository chatInvoiceRepository,
            IUserContextService userContextService,
            IPdfReportGenerator pdfReportGenerator,
            IMensajeProgramadoRepository mensajeProgramadoRepository)
        {
            _sessionRepository = sessionRepository;
            _openAIService = openAIService;
            _chatInvoiceRepository = chatInvoiceRepository;
            _userContextService = userContextService;
            _pdfReportGenerator = pdfReportGenerator;
            _mensajeProgramadoRepository = mensajeProgramadoRepository;
        }

        public async Task<ChatMessageResponse> ProcessMessageAsync(ChatMessageRequest request, CancellationToken cancellationToken)
        {
            // Obtener datos del cliente
            var userIdString = _userContextService.GetId();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in context");
            }

            var invoiceRecordsResult = await _chatInvoiceRepository.GetChatInvoiceByUserIdAsync(userIdString, cancellationToken);
            var invoiceRecords = invoiceRecordsResult.ToList();
            var customerData = FormatInvoiceData(invoiceRecords);
            var customerName = invoiceRecords.Count > 0 ? invoiceRecords[0].Customer.Trim() : null;

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

            var userMessage = request.Message.Trim();

            if (session.WaitingForPdf)
            {
                var msg = userMessage.ToLower();
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

                var confirms = confirmWords.Any(p => msg.Contains(p));
                var rejects = rejectWords.Any(p => msg.Contains(p));

                if (confirms)
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

                if (rejects)
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
            var mensajesProgramados = await _mensajeProgramadoRepository.GetMensajesActivosAsync(cancellationToken);

            var systemPrompt = BuildSystemPrompt(
                isFirstMessage,
                customerName,
                customerData,
                invoiceRecords.Count,
                mensajesProgramados
            );

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

            // Mantener solo los últimos 10 mensajes
            if (session.History.Count > 10)
            {
                session.History = session.History.Skip(session.History.Count - 10).ToList();
            }

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

        private static string BuildSystemPrompt(
            bool isFirstMessage,
            string? customerName,
            string customerData,
            int recordsCount,
            IEnumerable<MensajeProgramadoEntity> mensajesProgramados)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Eres un asistente virtual de Textiles Dislana.");
            sb.AppendLine("Respondes preguntas de clientes sobre facturas, saldos, pedidos, guías de envío y productos.");
            sb.AppendLine("Sé amable, claro y conciso. Responde siempre en español.");
            sb.AppendLine("No inventes información — usa solo los datos que se te proporcionan.");

            if (isFirstMessage && !string.IsNullOrEmpty(customerName))
            {
                sb.AppendLine($"Es el primer mensaje del cliente. Salúdalo por su nombre: \"{customerName}\" y dale la bienvenida a Textiles Dislana.");

                // Agregar mensajes programados al saludo
                var mensajesActivos = mensajesProgramados.Where(m => m.EsActivo()).ToList();
                if (mensajesActivos.Any())
                {
                    sb.AppendLine("IMPORTANTE: Después del saludo, agrega la siguiente información:");
                    foreach (var mensaje in mensajesActivos)
                    {
                        sb.AppendLine($"- {mensaje.Mensaje}");
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

