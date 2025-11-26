using Microsoft.EntityFrameworkCore;
using Libraray.Api.Context;
using Libraray.Api.DTO.Books;
using Libraray.Api.Entities;
using Library_backend.Repositories.Interfaces;

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

        public async Task<BorrowBookResponse?> BorrowBookAsync(Guid bookId, Guid userId, DateTime? dueDate)
        {
            // Use a transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Find the book with row locking to prevent race conditions
                var book = await _context.Books
                    .Where(b => b.Id == bookId)
                    .FirstOrDefaultAsync();

                if (book == null)
                {
                    return null; // Book not found
                }

                // Validate available copies
                if (book.AvailableCopies <= 0)
                {
                    return null; // No copies available
                }

                // Check if user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    return null; // User not found
                }

                // Decrease available copies
                book.AvailableCopies -= 1;
                book.UpdatedAt = DateTime.UtcNow;

                // Calculate due date (default 7 days from now if not provided)
                var calculatedDueDate = dueDate.HasValue 
                    ? DateOnly.FromDateTime(dueDate.Value) 
                    : DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

                // Create new borrowing record
                var borrowing = new Borrowing
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BookId = bookId,
                    BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    DueDate = calculatedDueDate,
                    ReturnDate = null,
                    Status = "Borrowed"
                };

                await _context.Borrowings.AddAsync(borrowing);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return new BorrowBookResponse
                {
                    AvailableCopies = book.AvailableCopies,
                    BorrowingId = borrowing.Id
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
