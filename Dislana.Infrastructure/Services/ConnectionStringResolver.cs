using Dislana.Application.Common.Interfaces;
using Dislana.Domain.Common.Enums;
using Microsoft.Extensions.Configuration;

namespace Dislana.Infrastructure.Services
{
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(DatabaseContext context)
        {
            return context switch
            {
                DatabaseContext.Ecommerce => GetEcommerceConnectionString(),
                DatabaseContext.ChatBot => GetChatConnectionString(),
                _ => throw new InvalidOperationException($"Database context '{context}' no está soportado")
            };
        }

        public string GetChatConnectionString()
        {
            return _configuration.GetConnectionString("ChatBotConnection")
                ?? throw new InvalidOperationException("Connection string 'ChatBotConnection' no está configurado");
        }

        private string GetEcommerceConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' no está configurado");
        }
    }
}


