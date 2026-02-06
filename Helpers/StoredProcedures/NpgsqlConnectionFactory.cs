using System.Data;
using Npgsql;

namespace Libraray.Api.Helpers.StoredProcedures
{
    /// <summary>
    /// PostgreSQL Connection Factory for Supabase
    /// Replaces SqlConnectionFactory for PostgreSQL compatibility
    /// </summary>
    public class NpgsqlConnectionFactory : IConnectionFactory
    {
   private readonly string _connectionString;

        public NpgsqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
      {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
