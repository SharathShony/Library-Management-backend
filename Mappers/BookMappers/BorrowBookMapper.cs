using System.Data;
using Libraray.Api.Helpers.StoredProcedures;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class BorrowBookMapper
    {
        /// <summary>
        /// Maps parameters for usp_BorrowBook stored procedure
        /// </summary>
        public static StoredProcedureParams<object> Parameters(
            Guid bookId, 
            Guid userId, 
            DateTime? dueDate)
        {
            var parameters = new StoredProcedureParams<object>("dbo.usp_BorrowBook");
            
            // Input parameters
            parameters.AddInputParameter("@book_id", bookId, DbType.Guid);
            parameters.AddInputParameter("@user_id", userId, DbType.Guid);
            parameters.AddInputParameter("@due_date", dueDate, DbType.DateTime2);
     
            // Output parameters
            parameters.AddOutputParameter("@borrowing_id", DbType.Guid);
            parameters.AddOutputParameter("@available_copies", DbType.Int32);
            parameters.AddOutputParameter("@error_code", DbType.Int32);
            parameters.AddOutputParameter("@error_message", DbType.String, 255);
            
            return parameters;
        }
    }
}
