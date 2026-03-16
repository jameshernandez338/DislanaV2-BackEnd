using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace Dislana.Infrastructure.Persistence.Dapper
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not configured.");
        }

        public DbConnection Create()
            => new SqlConnection(_connectionString);
    }
}
