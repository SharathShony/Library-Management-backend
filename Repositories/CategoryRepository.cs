using Libraray.Api.Context;
using Libraray.Api.DTO.Books;
using Libraray.Api.Entities;
using Libraray.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LibraryDbContext _context;

        public CategoryRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}
