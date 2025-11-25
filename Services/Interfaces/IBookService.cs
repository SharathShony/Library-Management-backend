using Libraray.Api.DTOs.Books;

namespace Libraray.Api.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookCatalogDto>> GetCatalogAsync();
    }
}
