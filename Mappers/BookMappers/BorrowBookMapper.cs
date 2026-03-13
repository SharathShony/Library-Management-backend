using System.Data;
using Libraray.Api.Helpers.StoredProcedures;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class BorrowBookMapper
    {
        /// <summary>
        /// Maps parameters for sp_borrow_book stored procedure
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
            
            return parameters;
        }

        /// <summary>
        /// Maps result set to BorrowBookResult
        /// </summary>
        public static Func<IDataReader, BorrowBookResult> ResultMapper() => reader => new BorrowBookResult
        {
            BorrowingId = reader.IsDBNull(0) ? (Guid?)null : reader.GetGuid(0),
            Status = reader.GetString(1),
            Message = reader.GetString(2)
        };
    }

    public class BorrowBookResult
    {
        public Guid? BorrowingId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
