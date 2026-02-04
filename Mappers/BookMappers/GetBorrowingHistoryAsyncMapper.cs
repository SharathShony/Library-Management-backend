using Libraray.Api.DTO.Books;
using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class GetBorrowingHistoryAsyncMapper
    {
        /// <summary>
        /// Maps userId parameter for usp_GetBorrowingHistory stored procedure
        /// </summary>
        public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
            new StoredProcedureParams<Guid>("dbo.usp_GetBorrowingHistory")
                .AddInputParameter("@user_id", userId, DbType.Guid);

        /// <summary>
        /// Maps SQL result to BorrowingHistoryDto
        /// </summary>
        public static Func<IDataReader, BorrowingHistoryDto> ResultMapper() => reader => new BorrowingHistoryDto
        {
            BorrowingId = reader.GetGuid(reader.GetOrdinal("borrowing_id")),
            BookId = reader.GetGuid(reader.GetOrdinal("book_id")),
            BookTitle = reader.GetString(reader.GetOrdinal("book_title")),
            Author = reader.IsDBNull(reader.GetOrdinal("author")) ? string.Empty : reader.GetString(reader.GetOrdinal("author")),
            BorrowDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("borrow_date"))),
            DueDate = reader.IsDBNull(reader.GetOrdinal("due_date")) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("due_date"))),
            ReturnDate = reader.IsDBNull(reader.GetOrdinal("return_date")) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("return_date"))),
            Status = reader.GetString(reader.GetOrdinal("status")),
            WasOverdue = reader.GetBoolean(reader.GetOrdinal("was_overdue"))
        };
    }
}
