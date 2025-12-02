using Libraray.Api.DTO.Books;

namespace Libraray.Api.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookCatalogDto>> GetCatalogAsync();
        Task<BookDetailsDto?> GetBookDetailsByIdAsync(Guid bookId);
        Task<BorrowBookResponse?> BorrowBookAsync(Guid bookId, Guid userId, DateTime? dueDate);
        Task<int> GetCurrentlyBorrowedCountAsync(Guid userId);
        Task<int> GetReturnedBooksCountAsync(Guid userId);
        Task<int> GetOverdueBooksCountAsync(Guid userId);
        Task<IEnumerable<BorrowedBookDto>> GetCurrentlyBorrowedBooksAsync(Guid userId);
        Task<ReturnBookResponse?> ReturnBookAsync(Guid borrowingId);
        Task<ExtendDueDateResponse?> ExtendDueDateAsync(Guid borrowingId, int extensionDays = 7);
        Task<IEnumerable<BorrowingHistoryDto>> GetBorrowingHistoryAsync(Guid userId);
    }
}
