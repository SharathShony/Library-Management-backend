using Libraray.Api.DTO.Books;
using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class GetCurrentlyBorrowedBooksAsyncMapper
    {
        /// <summary>
        /// Maps parameters for usp_GetCurrentlyBorrowedBooks stored procedure
        /// </summary>
        public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
            new StoredProcedureParams<Guid>("dbo.usp_GetCurrentlyBorrowedBooks")
                .AddInputParameter("@user_id", userId, DbType.Guid);

        /// <summary>
        /// Maps result set to BorrowedBookDto
        /// </summary>
        public static Func<IDataReader, BorrowedBookDto> ResultMapper() => reader => new BorrowedBookDto
        {
            BorrowingId = reader.GetGuid(reader.GetOrdinal("borrowing_id")),
            BookId = reader.GetGuid(reader.GetOrdinal("book_id")),
            BookTitle = reader.GetString(reader.GetOrdinal("book_title")),
            Author = reader.IsDBNull(reader.GetOrdinal("author")) ? string.Empty : reader.GetString(reader.GetOrdinal("author")),
            Isbn = reader.IsDBNull(reader.GetOrdinal("isbn")) ? null : reader.GetString(reader.GetOrdinal("isbn")),
            Publisher = reader.IsDBNull(reader.GetOrdinal("publisher")) ? null : reader.GetString(reader.GetOrdinal("publisher")),
            Summary = reader.IsDBNull(reader.GetOrdinal("summary")) ? null : reader.GetString(reader.GetOrdinal("summary")),
            BorrowDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("borrow_date"))),
            DueDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("due_date"))),
            IsOverdue = reader.GetBoolean(reader.GetOrdinal("is_overdue"))
        };
    }
}
