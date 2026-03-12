using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.DTO.Admin;
using System.Data;

namespace Libraray.Api.Mappers.AdminMappers
{
    public static class GetPendingBorrowingRequestsMapper
    {
        /// <summary>
        /// No parameters needed for getting all pending requests
    /// </summary>
      public static StoredProcedureParams<object> Parameters() =>
 new StoredProcedureParams<object>("sp_get_pending_borrowing_requests");

        /// <summary>
   /// Maps database reader to PendingBorrowingRequestDto
/// </summary>
        public static Func<IDataReader, PendingBorrowingRequestDto> ResultMapper() => reader => new PendingBorrowingRequestDto
  {
       BorrowingId = reader.GetGuid(reader.GetOrdinal("borrowing_id")),
   UserName = reader.GetString(reader.GetOrdinal("user_name")),
  UserEmail = reader.GetString(reader.GetOrdinal("user_email")),
      BookTitle = reader.GetString(reader.GetOrdinal("book_title")),
   BookId = reader.GetGuid(reader.GetOrdinal("book_id")),
     RequestedDate = reader.GetDateTime(reader.GetOrdinal("requested_date")),
  DueDate = reader.GetDateTime(reader.GetOrdinal("due_date"))
 };
    }
}
