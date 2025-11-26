using Libraray.Api.DTO.Books;
using Libraray.Api.DTOs.Books;

namespace Libraray.Api.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookCatalogDto>> GetCatalogAsync();
        Task<BookDetailsDto?> GetBookDetailsByIdAsync(Guid bookId);
    }
}
