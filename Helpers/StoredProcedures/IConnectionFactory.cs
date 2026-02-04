using System.Data;

namespace Libraray.Api.Helpers.StoredProcedures
{
    public interface IConnectionFactory
    {
      IDbConnection CreateConnection();
    }
}
