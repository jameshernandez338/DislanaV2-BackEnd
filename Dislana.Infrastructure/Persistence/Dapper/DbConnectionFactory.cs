using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace Dislana.Infrastructure.Persistence.Dapper
{
    public class DbConnectionFactory
    {
        private readonly string _defaultConnectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _defaultConnectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not configured.");
        }

        /// <summary>
        /// Crea una conexión usando la cadena de conexión por defecto (Portal)
        /// </summary>
        public DbConnection Create()
            => new SqlConnection(_defaultConnectionString);

        /// <summary>
        /// Crea una conexión usando una cadena de conexión específica
        /// </summary>
        public DbConnection Create(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión no puede estar vacía", nameof(connectionString));

            return new SqlConnection(connectionString);
        }
    }
}
