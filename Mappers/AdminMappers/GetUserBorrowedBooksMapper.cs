using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.DTO.Admin;
using System.Data;

namespace Libraray.Api.Mappers.AdminMappers
{
    public static class GetUserBorrowedBooksMapper
 {
   /// <summary>
    /// Maps userId parameter for stored procedure
   /// </summary>
        public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
new StoredProcedureParams<Guid>("sp_get_user_borrowed_books")
   .AddInputParameter("p_user_id", userId, DbType.Guid);

   /// <summary>
        /// Maps database reader to UserBorrowedBookDto
        /// </summary>
      public static Func<IDataReader, UserBorrowedBookDto> ResultMapper() => reader => new UserBorrowedBookDto
        {
   BorrowingId = reader.GetGuid(reader.GetOrdinal("borrowing_id")),
            BookId = reader.GetGuid(reader.GetOrdinal("book_id")),
  BookTitle = reader.GetString(reader.GetOrdinal("book_title")),
       Isbn = reader.IsDBNull(reader.GetOrdinal("isbn")) ? null : reader.GetString(reader.GetOrdinal("isbn")),
            BorrowedDate = reader.GetDateTime(reader.GetOrdinal("borrowed_date")),
DueDate = reader.GetDateTime(reader.GetOrdinal("due_date")),
            ReturnedDate = reader.IsDBNull(reader.GetOrdinal("returned_date")) 
     ? null 
    : reader.GetDateTime(reader.GetOrdinal("returned_date")),
 Status = reader.GetString(reader.GetOrdinal("status"))
  };
    }
}
