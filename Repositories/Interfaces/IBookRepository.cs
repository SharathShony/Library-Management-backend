using Libraray.Api.DTO.Books;
using System.Threading.Tasks;

namespace Library_backend.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookCatalogDto>> GetBookCatalogAsync();
        Task<BookDetailsDto?> GetBookDetailsByIdAsync(Guid bookId);
        Task<BorrowBookResponse?> BorrowBookAsync(Guid bookId, Guid userId, DateTime? dueDate);
        Task<int> GetCurrentlyBorrowedCountAsync(Guid userId);
        Task<int> GetReturnedBooksCountAsync(Guid userId);
        Task<int> GetOverdueBooksCountAsync(Guid userId);
        Task<IEnumerable<BorrowedBookDto>> GetCurrentlyBorrowedBooksAsync(Guid userId);
    }
}
