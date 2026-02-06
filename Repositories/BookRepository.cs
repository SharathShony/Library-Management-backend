using Libraray.Api.Context;
using Libraray.Api.DTO.Admin;
using Libraray.Api.DTO.Books;
using Libraray.Api.DTO.Users;
using Libraray.Api.Entities;
using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.Mappers.BookMappers;
using Libraray.Api.Mappers.UserMappers;
using Library_backend.Repositories.Interfaces;
// using Microsoft.EntityFrameworkCore; // EF Core - No longer needed for stored procedures

namespace Library_backend.Repositories
{
    public class BookRepository : IBookRepository
    {
   // private readonly LibraryDbContext _context; // EF Core DbContext - Kept for potential future use
        private readonly IConnectionFactory _connectionFactory;

        public BookRepository(LibraryDbContext context, IConnectionFactory connectionFactory)
        {
         // _context = context; // EF Core - Kept for potential future use
     _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<BookCatalogDto>> GetBookCatalogAsync()
        {
            //return await _context.Books
            //    .Include(b => b.BookAuthors)
            //        .ThenInclude(ba => ba.Author)
            //    .Include(b => b.BookCategories)
            //        .ThenInclude(bc => bc.Category)
            //    .Select(b => new BookCatalogDto
            //    {
            //        BookId = b.Id,
            //        Title = b.Title,
            //        Subtitle = b.Subtitle,
            //        Publisher = b.Publisher ?? "Unknown",
            //        Isbn = b.Isbn,
            //        Authors = b.BookAuthors
            //                    .Select(ba => ba.Author.Name)
            //                    .ToList(),
            //        Categories = b.BookCategories
            //                    .Select(bc => bc.Category.Name)
            //                    .ToList(),
            //        IsAvailable = b.AvailableCopies > 0
            //    })
            //    .ToListAsync();
            var parameters = GetBookCatalogAsyncMapper.Parameters();
            var resultMapper = GetBookCatalogAsyncMapper.ResultMapper();
            var results = await RepositoryHelper.ExecuteQueryAsync<string, BookCatalogDto>( _connectionFactory,parameters,resultMapper);
            return results;
        }

        public async Task<BookDetailsDto?> GetBookDetailsByIdAsync(Guid bookId)
        {
            //return await _context.Books
            //    .Include(b => b.BookAuthors)
            //        .ThenInclude(ba => ba.Author)
            //    .Include(b => b.BookCategories)
            //        .ThenInclude(bc => bc.Category)
            //    .Where(b => b.Id == bookId)
            //    .Select(b => new BookDetailsDto
            //    {
            //        BookId = b.Id, done
            //        Title = b.Title,done
            //        Subtitle = b.Subtitle,done
            //        Isbn = b.Isbn,done
            //        Summary = b.Summary,done
            //        Publisher = b.Publisher,done
            //        PublicationDate = b.PublicationDate,done
            //        TotalCopies = b.TotalCopies,done
            //        AvailableCopies = b.AvailableCopies,done
            //        IsAvailable = b.AvailableCopies > 0,done
            //        Authors = b.BookAuthors
            //                    .Select(ba => ba.Author.Name)
            //                    .ToList(),
            //        Categories = b.BookCategories
            //                    .Select(bc => bc.Category.Name)
            //                    .ToList(),
            //        CreatedAt = b.CreatedAt,
            //        UpdatedAt = b.UpdatedAt
            //    })
            //    .FirstOrDefaultAsync();
            var parameters = GetBookCatalogIdAsyncMapper.Parameters(bookId);
            var resultMapper = GetBookCatalogIdAsyncMapper.ResultMapper();
            var results = await RepositoryHelper.ExecuteQueryAsync<Guid, BookDetailsDto>( _connectionFactory,parameters,resultMapper);
            return results.FirstOrDefault();
        }

        public async Task<BorrowBookResponse?> BorrowBookAsync(Guid bookId, Guid userId, DateTime? dueDate)
        {
            try
            {
            var parameters = BorrowBookMapper.Parameters(bookId, userId, dueDate);
            var outputValues = await RepositoryHelper.ExecuteNonQueryWithOutputAsync(_connectionFactory, parameters);

            var errorCode = outputValues.ContainsKey("@error_code") && outputValues["@error_code"] != null ? Convert.ToInt32(outputValues["@error_code"]): 0;

            if (errorCode != 0)
              {
                return null;
              }

                var borrowingId = outputValues.ContainsKey("@borrowing_id") && outputValues["@borrowing_id"] != null? (Guid)outputValues["@borrowing_id"]: Guid.Empty;

                 var availableCopies = outputValues.ContainsKey("@available_copies") && outputValues["@available_copies"] != null ? Convert.ToInt32(outputValues["@available_copies"]): 0;

                return new BorrowBookResponse
                {
                    BorrowingId = borrowingId,
                    AvailableCopies = availableCopies,
                    Message = "Borrowing request submitted successfully! Waiting for admin approval.",
                    Status = "pending"
                };
            }
            catch
            {
                return null;
                }
        }

        public async Task<int> GetCurrentlyBorrowedCountAsync(Guid userId)
        {
          //return await _context.Borrowings
          //.Where(b => b.UserId == userId && b.Status == "approved" && b.ReturnDate == null)
          //.CountAsync();
          var parameters = GetCurrentlyBorrowedCountAsyncMapper.Parameters(userId);
          var result = await RepositoryHelper.ExecuteScalarAsync<Guid, int>(_connectionFactory, parameters);
          return result.HasValue ? result.Value : 0;
        } 

        public async Task<int> GetReturnedBooksCountAsync(Guid userId)
        {
            // A book is considered returned if ReturnDate is not null
            //return await _context.Borrowings
            //    .Where(b => b.UserId == userId && b.ReturnDate != null)
            //    .CountAsync();
            var parameters = GetReturnedCountAsyncMapper.Parameters(userId);
            var result = await RepositoryHelper.ExecuteScalarAsync<Guid, int>(_connectionFactory, parameters);
            return result.HasValue ? result.Value : 0;
        }


        public async Task<int> GetOverdueBooksCountAsync(Guid userId)
        {
            // A book is overdue if it is not returned and due date is before today
            //var today = DateOnly.FromDateTime(DateTime.UtcNow);
            //return await _context.Borrowings
            //    .Where(b => b.UserId == userId && b.ReturnDate == null && b.DueDate < today)
            //    .CountAsync();
            var parameters = GetOverdueBooksCountAsyncMapper.Parameters(userId);
            var result = await RepositoryHelper.ExecuteScalarAsync<Guid, int>(_connectionFactory, parameters);
            return result.HasValue ? result.Value : 0;
        }

        public async Task<IEnumerable<BorrowedBookDto>> GetCurrentlyBorrowedBooksAsync(Guid userId)
        {
            //var today = DateOnly.FromDateTime(DateTime.UtcNow);
  
            //return await _context.Borrowings
            //.Include(b => b.Book)
            //.ThenInclude(book => book.BookAuthors)
            //.ThenInclude(ba => ba.Author)
            //.Where(b => b.UserId == userId && b.Status == "approved" && b.ReturnDate == null)
            //.Select(b => new BorrowedBookDto
            // {
            //       BorrowingId = b.Id,
            //       BookId = b.BookId,
            //       BookTitle = b.Book.Title,
            //       Author = string.Join(", ", b.Book.BookAuthors.Select(ba => ba.Author.Name)),
            //       Isbn = b.Book.Isbn,
            //       Publisher = b.Book.Publisher,
            //       Summary = b.Book.Summary,
            //       BorrowDate = b.BorrowDate,
            //       DueDate = b.DueDate!.Value,
            //       IsOverdue = b.DueDate.HasValue && b.DueDate.Value < today
            //}).ToListAsync();
            var parameters = GetCurrentlyBorrowedBooksAsyncMapper.Parameters(userId);
            var resultMapper = GetCurrentlyBorrowedBooksAsyncMapper.ResultMapper();
            var results = await RepositoryHelper.ExecuteQueryAsync<Guid, BorrowedBookDto>(_connectionFactory, parameters, resultMapper);
            return results;
        }

        public async Task<ReturnBookResponse?> ReturnBookAsync(Guid borrowingId)
        {
            // New stored procedure implementation:
            try
  {
      var parameters = ReturnBookAsyncMapper.Parameters(borrowingId);
     var outputValues = await RepositoryHelper.ExecuteNonQueryWithOutputAsync(_connectionFactory, parameters);

     var errorCode = outputValues.ContainsKey("@error_code") && outputValues["@error_code"] != null ? Convert.ToInt32(outputValues["@error_code"]): 0;

     if (errorCode != 0)
        {
            return null;
        }

        var bookId = outputValues.ContainsKey("@book_id") && outputValues["@book_id"] != null ? (Guid)outputValues["@book_id"]: Guid.Empty;

        var returnDate = outputValues.ContainsKey("@return_date") && outputValues["@return_date"] != null ? DateOnly.FromDateTime((DateTime)outputValues["@return_date"]): DateOnly.MinValue;

         var availableCopies = outputValues.ContainsKey("@available_copies") && outputValues["@available_copies"] != null ? Convert.ToInt32(outputValues["@available_copies"]): 0;

        return new ReturnBookResponse
  {
        BorrowingId = borrowingId,
        BookId = bookId,
        ReturnDate = returnDate,
        AvailableCopies = availableCopies,
        Message = "Book returned successfully"
     };
    }
    catch
  {
        throw;
    }
}

        public async Task<ExtendDueDateResponse?> ExtendDueDateAsync(Guid borrowingId, int extensionDays = 7)
        {
            // Old EF Core code:
            //using var transaction = await _context.Database.BeginTransactionAsync();
            //
 //try
    //{
    //    var borrowing = await _context.Borrowings
    //        .Where(b => b.Id == borrowingId)
    //    .FirstOrDefaultAsync();
    //
    //    if (borrowing == null)
    //    {
    //        return null;
    //    }
    //
    //    if (borrowing.ReturnDate != null)
    //    {
    //        return null;
    //    }
    //
    //if (!borrowing.DueDate.HasValue)
  //    {
    //    return null;
    //    }
    //
    //    var oldDueDate = borrowing.DueDate.Value;
  //  borrowing.DueDate = oldDueDate.AddDays(extensionDays);
    //
    //    await _context.SaveChangesAsync();
    //    await transaction.CommitAsync();
    //
    //    return new ExtendDueDateResponse
    //    {
    //        BorrowingId = borrowing.Id,
    //        NewDueDate = borrowing.DueDate.Value,
    //        ExtensionDays = extensionDays,
    //        Message = "Due date extended successfully"
    //    };
    //}
    //catch
    //{
    //    await transaction.RollbackAsync();
    //    throw;
    //}

    // New stored procedure implementation:
 try
    {
   var parameters = ExtendDueDateAsyncMapper.Parameters(borrowingId, extensionDays);
        var outputValues = await RepositoryHelper.ExecuteNonQueryWithOutputAsync(_connectionFactory, parameters);

        var errorCode = outputValues.ContainsKey("@error_code") && outputValues["@error_code"] != null
            ? Convert.ToInt32(outputValues["@error_code"])
      : 0;

      if (errorCode != 0)
        {
            return null;
        }

     var newDueDate = outputValues.ContainsKey("@new_due_date") && outputValues["@new_due_date"] != null
         ? DateOnly.FromDateTime((DateTime)outputValues["@new_due_date"]): DateOnly.MinValue;

        return new ExtendDueDateResponse
        {
            BorrowingId = borrowingId,
            NewDueDate = newDueDate,
            ExtensionDays = extensionDays,
            Message = "Due date extended successfully"
        };
    }
    catch
    {
        return null;
    }
}

        public async Task<IEnumerable<BorrowingHistoryDto>> GetBorrowingHistoryAsync(Guid userId)
        {
         //   return await _context.Borrowings
         //   .Include(b => b.Book)
         //   .ThenInclude(book => book.BookAuthors)
         //   .ThenInclude(ba => ba.Author)
         //   .Where(b => b.UserId == userId && b.ReturnDate != null && b.Status == "Returned")
         //   .OrderByDescending(b => b.ReturnDate)
         //   .Select(b => new BorrowingHistoryDto
         //   {
         //   BorrowingId = b.Id,
         //   BookId = b.BookId,
         //   BookTitle = b.Book.Title,
         //   Author = string.Join(", ", b.Book.BookAuthors.Select(ba => ba.Author.Name)),
         //   BorrowDate = b.BorrowDate,
         //   DueDate = b.DueDate,
         //   ReturnDate = b.ReturnDate,
         //   Status = b.Status,
         //   WasOverdue = b.ReturnDate.HasValue && b.DueDate.HasValue && b.ReturnDate.Value > b.DueDate.Value
         //   })
         //.ToListAsync();
         var parameters = GetBorrowingHistoryAsyncMapper.Parameters(userId);
         var resultMapper = GetBorrowingHistoryAsyncMapper.ResultMapper();
         var results = await RepositoryHelper.ExecuteQueryAsync<Guid, BorrowingHistoryDto>(_connectionFactory, parameters, resultMapper);
         return results;
        }

        public async Task<CreateBookResponse?> CreateBookAsync(CreateBookRequest request)
{
   // Old EF Core code - COMMENTED OUT:
    //using var transaction = await _context.Database.BeginTransactionAsync();
    //
    //try
    //{
    //    // Create the book (EF Core entity)
    //    var book = new Book
  //    {
    //      Id = Guid.NewGuid(),
    //        Title = request.Title,
    //        Subtitle = request.Subtitle,
    //        Isbn = request.Isbn,
    //        Summary = request.Summary,
    //        Publisher = request.Publisher,
    // PublicationDate = request.PublicationDate,
    //        TotalCopies = request.TotalCopies,
    //   AvailableCopies = request.TotalCopies,
    //        CreatedAt = DateTime.UtcNow,
    //        UpdatedAt = DateTime.UtcNow
    //    };
    //
    //    await _context.Books.AddAsync(book); // EF Core: Add to DbSet
    //    await _context.SaveChangesAsync(); // EF Core: Save to database
    //
    //    // Handle authors
 //    if (request.Authors != null && request.Authors.Any())
    //    {
    //        foreach (var authorName in request.Authors)
    //        {
    //   // Check if author exists (EF Core query)
    //    var author = await _context.Authors
    //     .FirstOrDefaultAsync(a => a.Name == authorName);
    //
    //       // Create author if doesn't exist
    //    if (author == null)
    //        {
    //    author = new Author
    //        {
    //         Id = Guid.NewGuid(),
    //    Name = authorName
  //      };
    //              await _context.Authors.AddAsync(author); // EF Core: Add author
 //           await _context.SaveChangesAsync(); // EF Core: Save author
    //    }
    //
    //        // Create book-author relationship (EF Core entity)
    //   var bookAuthor = new BookAuthor
    //          {
    //          Id = Guid.NewGuid(),
    //          BookId = book.Id,
    //         AuthorId = author.Id
    //        };
    //   await _context.BookAuthors.AddAsync(bookAuthor); // EF Core: Add relationship
    //        }
    //    }
 //
    //    // Handle categories
    //    if (request.Categories != null && request.Categories.Any())
    //    {
    //     foreach (var categoryName in request.Categories)
    //        {
    //    // Check if category exists (EF Core query)
    //      var category = await _context.Categories
    //      .FirstOrDefaultAsync(c => c.Name == categoryName);
    //
    //          // Create category if doesn't exist
    //if (category == null)
    //            {
    //      category = new Category
    //      {
    //       Id = Guid.NewGuid(),
    //   Name = categoryName
    //    };
    //      await _context.Categories.AddAsync(category); // EF Core: Add category
    //    await _context.SaveChangesAsync(); // EF Core: Save category
    //   }
    //
    //            // Create book-category relationship (EF Core entity)
    //    var bookCategory = new BookCategory
    //            {
    //        Id = Guid.NewGuid(),
    // BookId = book.Id,
    //        CategoryId = category.Id
    //     };
    //         await _context.BookCategories.AddAsync(bookCategory); // EF Core: Add relationship
    //    }
    //    }
    //
    //  await _context.SaveChangesAsync(); // EF Core: Final save for all relationships
    //    await transaction.CommitAsync(); // EF Core: Commit transaction
    //
    //    return new CreateBookResponse
    //    {
    //        BookId = book.Id,
    //        Message = "Book created successfully"
    //    };
    //}
    //catch
    //{
    //    await transaction.RollbackAsync(); // EF Core: Rollback on error
    //    throw;
    //}

    // New stored procedure implementation:
    try
    {
        var parameters = CreateBookAsyncMapper.Parameters(request);
   var outputValues = await RepositoryHelper.ExecuteNonQueryWithJsonParamsAsync(_connectionFactory, parameters);

        var errorCode = outputValues.ContainsKey("@error_code") && outputValues["@error_code"] != null
            ? Convert.ToInt32(outputValues["@error_code"]): 0;

     if (errorCode != 0)
         {
        return null;
         }

        var newBookId = outputValues.ContainsKey("@new_book_id") && outputValues["@new_book_id"] != null
    ? (Guid)outputValues["@new_book_id"]
 : Guid.Empty;

        return new CreateBookResponse
        {
            BookId = newBookId,
            Message = "Book created successfully"
         };
         }
    catch
    {
        throw;
    }
}
public async Task<UpdateBookCopiesResponse?> UpdateBookCopiesAsync(Guid bookId, int totalCopies)
{
    // Old EF Core code:
    //using var transaction = await _context.Database.BeginTransactionAsync();
    //
    //try
    //{
    //    var book = await _context.Books
    //  .FirstOrDefaultAsync(b => b.Id == bookId);
    //
    //    if (book == null)
    //    {
    //        return null;
    //    }
    //
    //    // Calculate borrowed copies
    //    var borrowedCopies = book.TotalCopies - book.AvailableCopies;
    //
  //    // Validate that new total is at least as many as borrowed
    //    if (totalCopies < borrowedCopies)
    //    {
    //        return null; // Cannot reduce total copies below currently borrowed count
    //    }
    //
    //    // Update total and available copies
    //    book.TotalCopies = totalCopies;
    //    book.AvailableCopies = totalCopies - borrowedCopies;
    //    book.UpdatedAt = DateTime.UtcNow;
    //
    //    await _context.SaveChangesAsync();
    //    await transaction.CommitAsync();
    //
  //  return new UpdateBookCopiesResponse
    //    {
    //        TotalCopies = book.TotalCopies,
    //        AvailableCopies = book.AvailableCopies,
  //        Message = "Book copies updated successfully"
  //    };
    //}
    //catch
  //{
    //await transaction.RollbackAsync();
    //    throw;
    //}

    // New stored procedure implementation:
    try
    {
        var parameters = UpdateBookCopiesAsyncMapper.Parameters(bookId, totalCopies);
        var outputValues = await RepositoryHelper.ExecuteNonQueryWithOutputAsync(_connectionFactory, parameters);

        var errorCode = outputValues.ContainsKey("@error_code") && outputValues["@error_code"] != null
          ? Convert.ToInt32(outputValues["@error_code"])
   : 0;

        if (errorCode != 0)
   {
        // Error occurred - return null
            return null;
        }

        var newTotalCopies = outputValues.ContainsKey("@new_total_copies") && outputValues["@new_total_copies"] != null
            ? Convert.ToInt32(outputValues["@new_total_copies"])
            : 0;

        var newAvailableCopies = outputValues.ContainsKey("@new_available_copies") && outputValues["@new_available_copies"] != null
            ? Convert.ToInt32(outputValues["@new_available_copies"])
            : 0;

        return new UpdateBookCopiesResponse
        {
  TotalCopies = newTotalCopies,
        AvailableCopies = newAvailableCopies,
        Message = "Book copies updated successfully"
        };
    }
    catch
    {
        return null;
    }
}

        public async Task<bool> DeleteBookAsync(Guid bookId)
        {
  //using var transaction = await _context.Database.BeginTransactionAsync();
    //
    //try
    //{
    //    var book = await _context.Books
    //   .Include(b => b.BookAuthors)
    //        .Include(b => b.BookCategories)
    //        .Include(b => b.Borrowings)
    //        .FirstOrDefaultAsync(b => b.Id == bookId);
    //
    //    if (book == null)
    //    {
    //        return false;
    //    }
    //
    //    // Check if there are active borrowings
  //    var hasActiveBorrowings = book.Borrowings.Any(b => b.ReturnDate == null);
    // if (hasActiveBorrowings)
    //    {
    //        return false; // Cannot delete book with active borrowings
    //    }
    //
    //    // Remove ALL borrowing history (returned borrowings)
    //    if (book.Borrowings.Any())
    //{
    //        _context.Borrowings.RemoveRange(book.Borrowings);
    //  }
    //
    //    // Remove relationships
   //    _context.BookAuthors.RemoveRange(book.BookAuthors);
    //    _context.BookCategories.RemoveRange(book.BookCategories);
    //
    //    // Remove the book
    //    _context.Books.Remove(book);
    //
    //    await _context.SaveChangesAsync();
    //    await transaction.CommitAsync();
    //
 //    return true;
    //}
    //catch
    //{
 //    await transaction.RollbackAsync();
    //    throw;
    //}

    // New stored procedure implementation:
    try
  {
   var parameters = DeleteBookAsyncMapper.Parameters(bookId);
        var outputValues = await RepositoryHelper.ExecuteNonQueryWithOutputAsync(_connectionFactory, parameters);

        var errorCode = outputValues.ContainsKey("@error_code") && outputValues["@error_code"] != null
        ? Convert.ToInt32(outputValues["@error_code"])
       : 0;

        // If error occurred, return false
    if (errorCode != 0)
        {
         // Error codes:
    // 1 = Book not found
     // 2 = Book has active borrowings
            // 999 = Generic error
       return false;
        }

        // Success - book deleted
        return true;
    }
    catch
    {
        throw;
    }
}

    public async Task<bool> BookTitleExistsAsync(string title)
    {
          if (string.IsNullOrWhiteSpace(title))
            {
                return false;
            }

            var parameters = BookTitleExistsAsyncMapper.Parameters(title);
            var result = await RepositoryHelper.ExecuteScalarAsync<string, int>(_connectionFactory, parameters);
    
            return result.HasValue && result.Value > 0;
    }

public async Task<IEnumerable<OverdueUserDto>> GetOverdueUsersAsync()
        {
            var parameters = GetOverdueUsersAsyncMapper.Parameters();
            var resultMapper = GetOverdueUsersAsyncMapper.ResultMapper();
            var results = await RepositoryHelper.ExecuteQueryAsync<string, OverdueUserDto>(_connectionFactory, parameters, resultMapper);
        return results;
        }

        public async Task<UserOverdueBooksDto?> GetUserOverdueBooksAsync(Guid userId)
        {
            var parameters = GetUserOverdueBooksAsyncMapper.Parameters(userId);
          var userMapper = GetUserOverdueBooksAsyncMapper.UserResultMapper();
       var overdueBookMapper = GetUserOverdueBooksAsyncMapper.OverdueBookResultMapper();

   var result = await RepositoryHelper.ExecuteMultipleResultSetsAsync<Guid, UserOverdueBooksDto, OverdueBookDetailDto>(
         _connectionFactory,
        parameters,
     userMapper,
        overdueBookMapper,
     (main, details) => main.OverdueBooks = details);

        return result;
    }
    }   
}
