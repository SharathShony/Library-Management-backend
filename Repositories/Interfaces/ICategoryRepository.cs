using Libraray.Api.Entities;
using Libraray.Api.DTO.Books;

namespace Libraray.Api.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    }
}
