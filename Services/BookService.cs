using Libraray.Api.DTO.Books;
using Libraray.Api.DTO.Admin;
using Libraray.Api.Services.Interfaces;
using Library_backend.Repositories.Interfaces;

namespace Libraray.Api.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BookCatalogDto>> GetCatalogAsync()
        {
            return await _bookRepository.GetBookCatalogAsync();
        }

        public async Task<BookDetailsDto?> GetBookDetailsByIdAsync(Guid bookId)
        {
            return await _bookRepository.GetBookDetailsByIdAsync(bookId);
        }

        public async Task<BorrowBookResponse?> BorrowBookAsync(Guid bookId, Guid userId, DateTime? dueDate)
        {
            return await _bookRepository.BorrowBookAsync(bookId, userId, dueDate);
        }

        public async Task<int> GetCurrentlyBorrowedCountAsync(Guid userId)
        {
            return await _bookRepository.GetCurrentlyBorrowedCountAsync(userId);
        }

        public async Task<int> GetReturnedBooksCountAsync(Guid userId)
        {
            return await _bookRepository.GetReturnedBooksCountAsync(userId);
        }

        public async Task<int> GetOverdueBooksCountAsync(Guid userId)
        {
            return await _bookRepository.GetOverdueBooksCountAsync(userId);
        }

        public async Task<IEnumerable<BorrowedBookDto>> GetCurrentlyBorrowedBooksAsync(Guid userId)
        {
            return await _bookRepository.GetCurrentlyBorrowedBooksAsync(userId);
        }

        public async Task<ReturnBookResponse?> ReturnBookAsync(Guid borrowingId)
        {
            return await _bookRepository.ReturnBookAsync(borrowingId);
        }

        public async Task<ExtendDueDateResponse?> ExtendDueDateAsync(Guid borrowingId, int extensionDays = 7)
        {
            return await _bookRepository.ExtendDueDateAsync(borrowingId, extensionDays);
        }

        public async Task<IEnumerable<BorrowingHistoryDto>> GetBorrowingHistoryAsync(Guid userId)
        {
            return await _bookRepository.GetBorrowingHistoryAsync(userId);
        }

        public async Task<CreateBookResponse?> CreateBookAsync(CreateBookRequest request)
        {
            return await _bookRepository.CreateBookAsync(request);
        }

        public async Task<UpdateBookCopiesResponse?> UpdateBookCopiesAsync(Guid bookId, int totalCopies)
        {
            return await _bookRepository.UpdateBookCopiesAsync(bookId, totalCopies);
        }

        public async Task<bool> DeleteBookAsync(Guid bookId)
        {
            return await _bookRepository.DeleteBookAsync(bookId);
        }

        public async Task<bool> CheckBookTitleExistsAsync(string title)
        {
            return await _bookRepository.BookTitleExistsAsync(title);
        }

        public async Task<IEnumerable<OverdueUserDto>> GetOverdueUsersAsync()
        {
            return await _bookRepository.GetOverdueUsersAsync();
        }

        public async Task<UserOverdueBooksDto?> GetUserOverdueBooksAsync(Guid userId)
        {
            return await _bookRepository.GetUserOverdueBooksAsync(userId);
        }
    }
}
