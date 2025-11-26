using Microsoft.EntityFrameworkCore;
using Libraray.Api.Context;
using Libraray.Api.DTOs.Books;
using Library_backend.Repositories.Interfaces;
using Libraray.Api.DTO.Books;

namespace Library_backend.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookCatalogDto>> GetBookCatalogAsync()
        {
            return await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Select(b => new BookCatalogDto
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    Publisher = b.Publisher ?? "Unknown",
                    Isbn = b.Isbn,
                    Authors = b.BookAuthors
                                .Select(ba => ba.Author.Name)
                                .ToList(),
                    Categories = b.BookCategories
                                .Select(bc => bc.Category.Name)
                                .ToList(),
                    IsAvailable = b.AvailableCopies > 0
                })
                .ToListAsync();
        }

        public async Task<BookDetailsDto?> GetBookDetailsByIdAsync(Guid bookId)
        {
            return await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Where(b => b.Id == bookId)
                .Select(b => new BookDetailsDto
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    Isbn = b.Isbn,
                    Summary = b.Summary,
                    Publisher = b.Publisher,
                    PublicationDate = b.PublicationDate,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies,
                    IsAvailable = b.AvailableCopies > 0,
                    Authors = b.BookAuthors
                                .Select(ba => ba.Author.Name)
                                .ToList(),
                    Categories = b.BookCategories
                                .Select(bc => bc.Category.Name)
                                .ToList(),
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }
    }
}
