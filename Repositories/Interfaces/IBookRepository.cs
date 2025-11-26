using Libraray.Api.DTO.Books;

namespace Library_backend.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookCatalogDto>> GetBookCatalogAsync();
        Task<BookDetailsDto?> GetBookDetailsByIdAsync(Guid bookId);
        Task<BorrowBookResponse?> BorrowBookAsync(Guid bookId, Guid userId, DateTime? dueDate);
    }
}
