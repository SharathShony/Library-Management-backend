using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class BookTitleExistsAsyncMapper
    {
        /// <summary>
   /// Maps title parameter for usp_BookTitleExists stored procedure
    /// </summary>
        public static StoredProcedureParams<string> Parameters(string title) =>
  new StoredProcedureParams<string>("dbo.usp_BookTitleExists")
           .AddInputParameter("@title", title, DbType.String);
    }
}
