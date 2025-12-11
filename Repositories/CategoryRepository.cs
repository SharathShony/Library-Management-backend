using Libraray.Api.Context;
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
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        }
    }
}
