using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class UpdateBookCopiesAsyncMapper
    {
        /// <summary>
        /// Maps parameters for usp_UpdateBookCopies stored procedure
        /// </summary>
        public static StoredProcedureParams<object> Parameters(Guid bookId, int totalCopies)
        {
            var parameters = new StoredProcedureParams<object>("usp_update_book_copies");
            
            // Input parameters
            parameters.AddInputParameter("p_book_id", bookId, DbType.Guid);
     parameters.AddInputParameter("p_total_copies", totalCopies, DbType.Int32);
   
       // Output parameters
            parameters.AddOutputParameter("new_total_copies", DbType.Int32);
       parameters.AddOutputParameter("new_available_copies", DbType.Int32);
  parameters.AddOutputParameter("error_code", DbType.Int32);
      parameters.AddOutputParameter("error_message", DbType.String, 255);
            
  return parameters;
        }
  }
}
