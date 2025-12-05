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

        public async Task<int> GetCurrentlyBorrowedCountAsync(Guid userId)
        {
            // Assuming "Borrowed" status means currently borrowed and not returned
            return await _context.Borrowings
                .Where(b => b.UserId == userId && b.Status == "Borrowed" && b.ReturnDate == null)
                .CountAsync();
        }

        public async Task<int> GetReturnedBooksCountAsync(Guid userId)
        {
            // A book is considered returned if ReturnDate is not null
            return await _context.Borrowings
                .Where(b => b.UserId == userId && b.ReturnDate != null)
                .CountAsync();
        }

        public async Task<int> GetOverdueBooksCountAsync(Guid userId)
        {
            // A book is overdue if it is not returned and due date is before today
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.Borrowings
     .Where(b => b.UserId == userId && b.ReturnDate == null && b.DueDate < today)
                .CountAsync();
        }

        public async Task<IEnumerable<BorrowedBookDto>> GetCurrentlyBorrowedBooksAsync(Guid userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
  
                 return await _context.Borrowings
                .Include(b => b.Book)
                .ThenInclude(book => book.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .Where(b => b.UserId == userId && b.ReturnDate == null && b.Status == "Borrowed")
                .Select(b => new BorrowedBookDto
                {
                                                 BorrowingId = b.Id,
                                                 BookId = b.BookId,
                                                 BookTitle = b.Book.Title,
                                                 Author = string.Join(", ", b.Book.BookAuthors.Select(ba => ba.Author.Name)),
                                                 Isbn = b.Book.Isbn,
                                                 Publisher = b.Book.Publisher,
                                                 Summary = b.Book.Summary,
                                                 BorrowDate = b.BorrowDate,
                                                 DueDate = b.DueDate!.Value,
                                                 IsOverdue = b.DueDate.HasValue && b.DueDate.Value < today
                }).ToListAsync();}

        public async Task<ReturnBookResponse?> ReturnBookAsync(Guid borrowingId)
        {
            // Use a transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Find the borrowing record
                var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.Id == borrowingId)
                .FirstOrDefaultAsync();

            if (borrowing == null)
             {
                  return null; // Borrowing not found
             }

          // Check if already returned
            if (borrowing.ReturnDate != null)
            {
           return null; // Book already returned
            }

                   // Update return date
            borrowing.ReturnDate = DateOnly.FromDateTime(DateTime.UtcNow);
            borrowing.Status = "Returned";

       // Increment available copies
             borrowing.Book.AvailableCopies += 1;
             borrowing.Book.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

     // Commit transaction
             await transaction.CommitAsync();

            return new ReturnBookResponse
            {
                BorrowingId = borrowing.Id,
                BookId = borrowing.BookId,
                ReturnDate = borrowing.ReturnDate.Value,
                AvailableCopies = borrowing.Book.AvailableCopies,
                Message = "Book returned successfully"
             };
            }
            catch
             {
                await transaction.RollbackAsync();
                throw;
                }
             }

        public async Task<ExtendDueDateResponse?> ExtendDueDateAsync(Guid borrowingId, int extensionDays = 7)
        {
            // Use a transaction to ensure data consistency
        using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
            // Find the borrowing record
            var borrowing = await _context.Borrowings
            .Where(b => b.Id == borrowingId)
            .FirstOrDefaultAsync();

            if (borrowing == null)
             {
                 return null; // Borrowing not found
             }

            // Check if book is already returned
             if (borrowing.ReturnDate != null)
              {
                return null; // Cannot extend due date for returned book
                 }

            // Check if due date exists
                 if (!borrowing.DueDate.HasValue)
                 {
                    return null; // No due date to extend
                  }

              // Extend the due date
               var oldDueDate = borrowing.DueDate.Value;
                borrowing.DueDate = oldDueDate.AddDays(extensionDays);

            await _context.SaveChangesAsync();

             // Commit transaction
                await transaction.CommitAsync();

            return new ExtendDueDateResponse
             {
            BorrowingId = borrowing.Id,
            NewDueDate = borrowing.DueDate.Value,
            ExtensionDays = extensionDays,
             Message = "Due date extended successfully"
                };
            }
            catch
            {
                 await transaction.RollbackAsync();
                 throw;
            }
   }

        public async Task<IEnumerable<BorrowingHistoryDto>> GetBorrowingHistoryAsync(Guid userId)
        {
            return await _context.Borrowings
.Include(b => b.Book)
              .ThenInclude(book => book.BookAuthors)
                .ThenInclude(ba => ba.Author)
       .Where(b => b.UserId == userId && b.ReturnDate != null && b.Status == "Returned")
          .OrderByDescending(b => b.ReturnDate)
  .Select(b => new BorrowingHistoryDto
          {
       BorrowingId = b.Id,
      BookId = b.BookId,
       BookTitle = b.Book.Title,
            Author = string.Join(", ", b.Book.BookAuthors.Select(ba => ba.Author.Name)),
   BorrowDate = b.BorrowDate,
            DueDate = b.DueDate,
       ReturnDate = b.ReturnDate,
Status = b.Status,
        WasOverdue = b.ReturnDate.HasValue && b.DueDate.HasValue && b.ReturnDate.Value > b.DueDate.Value
          })
         .ToListAsync();
        }

        public async Task<CreateBookResponse?> CreateBookAsync(CreateBookRequest request)
{
            using var transaction = await _context.Database.BeginTransactionAsync();

try
      {
        // Create the book
       var book = new Book
         {
        Id = Guid.NewGuid(),
         Title = request.Title,
       Subtitle = request.Subtitle,
  Isbn = request.Isbn,
 Summary = request.Summary,
   Publisher = request.Publisher,
   PublicationDate = request.PublicationDate,
     TotalCopies = request.TotalCopies,
         AvailableCopies = request.TotalCopies,
          CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
  };

      await _context.Books.AddAsync(book);
             await _context.SaveChangesAsync();

       // Handle authors
if (request.Authors != null && request.Authors.Any())
        {
     foreach (var authorName in request.Authors)
   {
          // Check if author exists
 var author = await _context.Authors
        .FirstOrDefaultAsync(a => a.Name == authorName);

      // Create author if doesn't exist
        if (author == null)
   {
        author = new Author
        {
Id = Guid.NewGuid(),
      Name = authorName
    };
              await _context.Authors.AddAsync(author);
       await _context.SaveChangesAsync();
       }

          // Create book-author relationship
      var bookAuthor = new BookAuthor
      {
           Id = Guid.NewGuid(),
 BookId = book.Id,
  AuthorId = author.Id
             };
        await _context.BookAuthors.AddAsync(bookAuthor);
             }
 }

       // Handle categories
                if (request.Categories != null && request.Categories.Any())
    {
         foreach (var categoryName in request.Categories)
        {
             // Check if category exists
     var category = await _context.Categories
        .FirstOrDefaultAsync(c => c.Name == categoryName);

            // Create category if doesn't exist
if (category == null)
            {
   category = new Category
     {
            Id = Guid.NewGuid(),
      Name = categoryName
          };
       await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
       }

                // Create book-category relationship
      var bookCategory = new BookCategory
                     {
                 Id = Guid.NewGuid(),
        BookId = book.Id,
       CategoryId = category.Id
};
await _context.BookCategories.AddAsync(bookCategory);
         }
      }

         await _context.SaveChangesAsync();
       await transaction.CommitAsync();

                return new CreateBookResponse
       {
 BookId = book.Id,
     Message = "Book created successfully"
        };
     }
    catch
    {
         await transaction.RollbackAsync();
    throw;
   }
        }

        public async Task<UpdateBookCopiesResponse?> UpdateBookCopiesAsync(Guid bookId, int totalCopies)
        {
          using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
          var book = await _context.Books
                 .FirstOrDefaultAsync(b => b.Id == bookId);

    if (book == null)
    {
       return null;
      }

        // Calculate borrowed copies
         var borrowedCopies = book.TotalCopies - book.AvailableCopies;

            // Validate that new total is at least as many as borrowed
     if (totalCopies < borrowedCopies)
                {
    return null; // Cannot reduce total copies below currently borrowed count
                }

        // Update total and available copies
    book.TotalCopies = totalCopies;
        book.AvailableCopies = totalCopies - borrowedCopies;
        book.UpdatedAt = DateTime.UtcNow;

 await _context.SaveChangesAsync();
  await transaction.CommitAsync();

         return new UpdateBookCopiesResponse
    {
        TotalCopies = book.TotalCopies,
              AvailableCopies = book.AvailableCopies,
                    Message = "Book copies updated successfully"
          };
    }
 catch
       {
      await transaction.RollbackAsync();
             throw;
   }
        }

        public async Task<bool> DeleteBookAsync(Guid bookId)
        {
  using var transaction = await _context.Database.BeginTransactionAsync();

    try
            {
                var book = await _context.Books
           .Include(b => b.BookAuthors)
                .Include(b => b.BookCategories)
         .Include(b => b.Borrowings)
           .FirstOrDefaultAsync(b => b.Id == bookId);

       if (book == null)
  {
       return false;
      }

  // Check if there are active borrowings
       var hasActiveBorrowings = book.Borrowings.Any(b => b.ReturnDate == null);
       if (hasActiveBorrowings)
        {
          return false; // Cannot delete book with active borrowings
                }

     // Remove relationships
        _context.BookAuthors.RemoveRange(book.BookAuthors);
           _context.BookCategories.RemoveRange(book.BookCategories);
         
     // Remove the book
          _context.Books.Remove(book);

     await _context.SaveChangesAsync();
await transaction.CommitAsync();

                return true;
       }
      catch
            {
    await transaction.RollbackAsync();
 throw;
            }
      }
    }   
}
