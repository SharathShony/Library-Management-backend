using Libraray.Api.DTO.Books;
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
    }
}
