using Libraray.Api.DTO.Books;
using Libraray.Api.Repositories.Interfaces;
using Libraray.Api.Services.Interfaces;

namespace Libraray.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name });
        }
    }
}
