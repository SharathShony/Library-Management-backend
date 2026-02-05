using Libraray.Api.DTO.Books;
using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class GetBookCatalogAsyncMapper
    {
        public static StoredProcedureParams<string> Parameters() =>
            new StoredProcedureParams<string>("sp_GetBookCatalog");

        public static Func<IDataReader, BookCatalogDto> ResultMapper() => reader => new BookCatalogDto
        {
            BookId = reader.GetGuid(reader.GetOrdinal("book_id")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            Subtitle = reader.IsDBNull(reader.GetOrdinal("subtitle"))? null: reader.GetString(reader.GetOrdinal("subtitle")),
            Publisher = reader.IsDBNull(reader.GetOrdinal("publisher"))? "Unknown": reader.GetString(reader.GetOrdinal("publisher")),
            Isbn = reader.IsDBNull(reader.GetOrdinal("isbn")) ? null: reader.GetString(reader.GetOrdinal("isbn")),
            Authors = reader.IsDBNull(reader.GetOrdinal("authors"))? new List<string>(): reader.GetString(reader.GetOrdinal("authors")).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList(),
            Categories = reader.IsDBNull(reader.GetOrdinal("categories"))? new List<string>(): reader.GetString(reader.GetOrdinal("categories")).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList(),
            IsAvailable = reader.GetInt32(reader.GetOrdinal("available_copies")) > 0
        };
    }
}
