using Libraray.Api.DTO.Books;

namespace Libraray.Api.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookCatalogDto>> GetCatalogAsync();
        Task<BookDetailsDto?> GetBookDetailsByIdAsync(Guid bookId);
        Task<BorrowBookResponse?> BorrowBookAsync(Guid bookId, Guid userId, DateTime? dueDate);
        Task<int> GetCurrentlyBorrowedCountAsync(Guid userId);
    }
}
