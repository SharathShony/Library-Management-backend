using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class DeleteBookAsyncMapper
    {
        public static StoredProcedureParams<Guid> Parameters(Guid bookId)
        {
            var parameters = new StoredProcedureParams<Guid>("usp_delete_book");
            
            // Input parameter
            parameters.AddInputParameter("p_book_id", bookId, DbType.Guid);
        
            // Output parameters
            parameters.AddOutputParameter("error_code", DbType.Int32);
            parameters.AddOutputParameter("error_message", DbType.String, 255);
   
            return parameters;
        }
    }
}
