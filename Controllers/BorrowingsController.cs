using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Libraray.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Libraray.Api.Context;
using System.Security.Claims;
using Libraray.Api.DTO.Books;

namespace Libraray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BorrowingsController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly LibraryDbContext _context;

        public BorrowingsController(IBookService bookService, LibraryDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        [HttpGet("currently-borrowed/count")]
        public async Task<IActionResult> GetCurrentlyBorrowedCount([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { message = "Invalid userId" });

            var count = await _bookService.GetCurrentlyBorrowedCountAsync(userId);
            return Ok(new { count });
        }

        [HttpGet("returned/count")]
        public async Task<IActionResult> GetReturnedBooksCount([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { message = "Invalid userId" });

            var count = await _bookService.GetReturnedBooksCountAsync(userId);
            return Ok(new { count });
        }

        [HttpGet("overdue/count")]
        public async Task<IActionResult> GetOverdueBooksCount([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { message = "Invalid userId" });

            var count = await _bookService.GetOverdueBooksCountAsync(userId);
            return Ok(new { count });
        }

        [HttpGet("currently-borrowed")]
        public async Task<IActionResult> GetCurrentlyBorrowedBooks([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { message = "Invalid userId" });

            var books = await _bookService.GetCurrentlyBorrowedBooksAsync(userId);
            return Ok(books);
        }

        [HttpPost("{borrowingId}/return")]
        public async Task<IActionResult> ReturnBook(Guid borrowingId)
        {
            if (borrowingId == Guid.Empty)
                return BadRequest(new { message = "Invalid borrowing ID" });

            try
            {
                var result = await _bookService.ReturnBookAsync(borrowingId);

                if (result == null)
                {
                    return NotFound(new { message = "Borrowing not found or book already returned" });
                }

                return Ok(result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Concurrency conflict occurred. Please try again." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while returning the book", error = ex.Message });
            }
        }

        [HttpPost("{borrowingId}/extend")]
        public async Task<IActionResult> ExtendDueDate(Guid borrowingId, [FromQuery] int extensionDays = 7)
        {
            if (borrowingId == Guid.Empty)
                return BadRequest(new { message = "Invalid borrowing ID" });

            if (extensionDays <= 0)
                return BadRequest(new { message = "Extension days must be greater than 0" });

            try
            {
                var result = await _bookService.ExtendDueDateAsync(borrowingId, extensionDays);

                if (result == null)
                {
                    return NotFound(new { message = "Borrowing not found, book already returned, or no due date to extend" });
                }

                return Ok(result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Concurrency conflict occurred. Please try again." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while extending the due date", error = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetBorrowingHistory([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { message = "Invalid userId" });

            var history = await _bookService.GetBorrowingHistoryAsync(userId);
            return Ok(history);
        }
        [HttpGet("check-title")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CheckBookTitle([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest(new { message = "Title parameter is required" });
            }

            var exists = await _bookService.CheckBookTitleExistsAsync(title);
            
            var message = exists 
                ? $"Book with title '{title}' already exists" 
                : $"Book with title '{title}' is available";

            return Ok(new { exists, message });
        }

        /// <summary>
        /// Get user's own pending borrowing requests
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetUserPendingRequests([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid userId" });
            }

            // Get current user ID from JWT token
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("sub")?.Value;
            
            // Ensure users can only see their own pending requests
            if (currentUserIdClaim != userId.ToString())
            {
                return Forbid();
            }

            var pendingRequests = await _context.Borrowings
                .Where(b => b.UserId == userId && b.Status == "pending")
                .Include(b => b.Book)
                    .ThenInclude(book => book.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .OrderByDescending(b => b.BorrowDate)
                .Select(b => new
                {
                    borrowingId = b.Id,
                    bookId = b.BookId,
                    bookTitle = b.Book.Title,
                    author = string.Join(", ", b.Book.BookAuthors.Select(ba => ba.Author.Name)),
                    isbn = b.Book.Isbn,
                    requestedDate = b.BorrowDate.ToDateTime(TimeOnly.MinValue),
                    dueDate = b.DueDate.HasValue ? b.DueDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                    status = b.Status
                })
                .ToListAsync();

            return Ok(pendingRequests);
        }
    }
}