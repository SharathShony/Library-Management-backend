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
        Task<ReturnBookResponse?> ReturnBookAsync(Guid borrowingId);
        Task<ExtendDueDateResponse?> ExtendDueDateAsync(Guid borrowingId, int extensionDays = 7);
        Task<IEnumerable<BorrowingHistoryDto>> GetBorrowingHistoryAsync(Guid userId);
        Task<CreateBookResponse?> CreateBookAsync(CreateBookRequest request);
        Task<UpdateBookCopiesResponse?> UpdateBookCopiesAsync(Guid bookId, int totalCopies);
        Task<bool> DeleteBookAsync(Guid bookId);
        Task<bool> BookTitleExistsAsync(string title);
    }
}
