using Libraray.Api.Context;
using Libraray.Api.DTO.Books;
using Libraray.Api.Entities;
using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.Mappers.CategoryMappers;
using Libraray.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LibraryDbContext _context;
        private readonly IConnectionFactory _connectionFactory;

        public CategoryRepository(LibraryDbContext context, IConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            //return await _context.Categories
            //    .OrderBy(c => c.Name)
            //    .Select(c => new CategoryDto
            //    {
            //        Id = c.Id,
            //        Name = c.Name
            //    })
            //    .ToListAsync();
            var parameters = GetAllCategoriesAsyncMapper.Parameters();
            var resultMapper = GetAllCategoriesAsyncMapper.ResultMapper();
            var results = await RepositoryHelper.ExecuteQueryAsync<string, CategoryDto>(_connectionFactory,parameters,resultMapper);

            return results;
        }
    }
}
