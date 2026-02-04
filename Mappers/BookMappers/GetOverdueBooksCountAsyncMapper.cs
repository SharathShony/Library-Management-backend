using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class GetOverdueBooksCountAsyncMapper
    {
        /// <summary>
        /// Maps parameters for usp_GetOverdueBooksCount stored procedure
    /// </summary>
   public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
            new StoredProcedureParams<Guid>("dbo.usp_GetOverdueBooksCount")
           .AddInputParameter("@user_id", userId, DbType.Guid);
  }
}
