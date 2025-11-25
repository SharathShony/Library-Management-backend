using Libraray.Api.DTOs.Books;

namespace Library_backend.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookCatalogDto>> GetBookCatalogAsync();
    }
}
