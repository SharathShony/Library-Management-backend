using Libraray.Api.DTO.Books;
using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class GetBookCatalogIdAsyncMapper
    {
        /// <summary>
        /// Maps book ID to stored procedure parameters
        /// </summary>
        public static StoredProcedureParams<Guid> Parameters(Guid bookId) =>
            new StoredProcedureParams<Guid>("sp_get_book_details_by_id")
            .AddInputParameter("p_book_id", bookId);

        /// <summary>
        /// Maps database reader to BookDetailsDto
        /// </summary>
        public static Func<IDataReader, BookDetailsDto> ResultMapper() => reader => new BookDetailsDto
        {
            BookId = reader.GetGuid(reader.GetOrdinal("book_id")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            Subtitle = reader.IsDBNull(reader.GetOrdinal("subtitle")) ? null : reader.GetString(reader.GetOrdinal("subtitle")),
            Publisher = reader.IsDBNull(reader.GetOrdinal("publisher")) ? null : reader.GetString(reader.GetOrdinal("publisher")),
            Isbn = reader.IsDBNull(reader.GetOrdinal("isbn")) ? null : reader.GetString(reader.GetOrdinal("isbn")),
            Summary = reader.IsDBNull(reader.GetOrdinal("summary")) ? null : reader.GetString(reader.GetOrdinal("summary")),
            PublicationDate = reader.IsDBNull(reader.GetOrdinal("publication_date")) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("publication_date"))),
            TotalCopies = reader.GetInt32(reader.GetOrdinal("total_copies")),
            AvailableCopies = reader.GetInt32(reader.GetOrdinal("available_copies")),
            IsAvailable = reader.GetInt32(reader.GetOrdinal("available_copies")) > 0,
            Authors = reader.IsDBNull(reader.GetOrdinal("authors")) ? new List<string>() : reader.GetString(reader.GetOrdinal("authors")).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList(),
            Categories = reader.IsDBNull(reader.GetOrdinal("categories")) ? new List<string>() : reader.GetString(reader.GetOrdinal("categories")).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList(),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}
