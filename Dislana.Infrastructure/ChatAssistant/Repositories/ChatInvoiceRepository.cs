using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.Common.Enums;
using Dislana.Infrastructure.ChatAssistant.DTOs;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.ChatAssistant.Repositories
{
    public class ChatInvoiceRepository : IChatInvoiceRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.ChatBot;

        public ChatInvoiceRepository(IContextualDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<IEnumerable<ChatInvoiceEntity>> GetChatInvoiceByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var query = @"
                SELECT 
                    LTRIM(RTRIM(TypeDocument)) AS TypeDocument,
                    LTRIM(RTRIM(Number)) AS Number,
                    CONVERT(varchar, fecha, 103) AS Fecha,
                    LTRIM(RTRIM(CustomerDni)) AS CustomerDni,
                    LTRIM(RTRIM(Customer)) AS Customer,
                    valor AS Valor,
                    saldo AS Saldo,
                    linkInvoice AS LinkInvoice,
                    LTRIM(RTRIM(guia)) AS Guia,
                    linkguia AS LinkGuia,
                    enviado AS Enviado,
                    enviadoguia AS EnviadoGuia
                FROM dbo.contactos
                WHERE login = @userId
                ORDER BY fecha DESC";

            var parameters = new { userId };
            var dtos = await _dbExecutor.QueryAsync<ChatInvoiceDto>(Context, query, parameters, null, cancellationToken);

            return dtos.Select(dto => new ChatInvoiceEntity
            {
                TypeDocument = dto.TypeDocument,
                Number = dto.Number,
                Fecha = dto.Fecha,
                CustomerDni = dto.CustomerDni,
                Customer = dto.Customer,
                Valor = dto.Valor,
                Saldo = dto.Saldo,
                LinkInvoice = dto.LinkInvoice,
                Guia = dto.Guia,
                LinkGuia = dto.LinkGuia,
                Enviado = dto.Enviado,
                EnviadoGuia = dto.EnviadoGuia
            });
        }
    }
}

