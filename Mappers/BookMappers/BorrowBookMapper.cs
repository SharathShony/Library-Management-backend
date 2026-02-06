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
            var parameters = new StoredProcedureParams<object>("usp_borrow_book");
            
            // Input parameters
            parameters.AddInputParameter("p_book_id", bookId, DbType.Guid);
            parameters.AddInputParameter("p_user_id", userId, DbType.Guid);
            parameters.AddInputParameter("p_due_date", dueDate, DbType.DateTime2);
     
            // Output parameters
            parameters.AddOutputParameter("borrowing_id", DbType.Guid);
            parameters.AddOutputParameter("available_copies", DbType.Int32);
            parameters.AddOutputParameter("error_code", DbType.Int32);
            parameters.AddOutputParameter("error_message", DbType.String, 255);
            
            return parameters;
        }
    }
}
