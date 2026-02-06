using Libraray.Api.DTO.Admin;
using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class GetUserOverdueBooksAsyncMapper
    {
        /// <summary>
        /// Maps userId parameter for usp_GetUserOverdueBooks stored procedure
   /// </summary>
        public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
        new StoredProcedureParams<Guid>("usp_get_user_overdue_books")
     .AddInputParameter("p_user_id", userId, DbType.Guid);

        /// <summary>
        /// Maps first result set (user info) to UserOverdueBooksDto
        /// </summary>
      public static Func<IDataReader, UserOverdueBooksDto> UserResultMapper() => reader => new UserOverdueBooksDto
        {
       UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
        UserName = reader.GetString(reader.GetOrdinal("user_name")),
            Email = reader.GetString(reader.GetOrdinal("email")),
        OverdueBooks = new List<OverdueBookDetailDto>()
        };

        /// <summary>
    /// Maps second result set (overdue books) to OverdueBookDetailDto
        /// </summary>
        public static Func<IDataReader, OverdueBookDetailDto> OverdueBookResultMapper() => reader => new OverdueBookDetailDto
        {
    BorrowingId = reader.GetGuid(reader.GetOrdinal("borrowing_id")),
     BookId = reader.GetGuid(reader.GetOrdinal("book_id")),
          BookTitle = reader.GetString(reader.GetOrdinal("book_title")),
     BorrowedDate = reader.GetDateTime(reader.GetOrdinal("borrowed_date")),
            DueDate = reader.GetDateTime(reader.GetOrdinal("due_date")),
            DaysOverdue = reader.GetInt32(reader.GetOrdinal("days_overdue"))
  };
    }
}
