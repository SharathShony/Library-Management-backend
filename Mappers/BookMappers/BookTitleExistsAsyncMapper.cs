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
  new StoredProcedureParams<string>("usp_book_title_exists")
           .AddInputParameter("p_title", title, DbType.String);
    }
}
