using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Libraray.Api.DTO.Books;
using Libraray.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("catalog")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetCatalog()
        {
            var books = await _bookService.GetCatalogAsync();
            return Ok(books);
        }

        [HttpGet("{bookId}/details")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetBookDetails(Guid bookId)
        {
            var book = await _bookService.GetBookDetailsByIdAsync(bookId);
            
            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            return Ok(book);
        }

        [HttpGet("check-title")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> CheckBookTitle([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest(new { exists = false, message = "Title is required" });
            }

            var exists = await _bookService.CheckBookTitleExistsAsync(title);

            if (exists)
            {
                return Ok(new { exists = true, message = "A book with this title already exists" });
            }

            return Ok(new { exists = false });
        }
            
        [HttpPost("{bookId}/borrow")]
        public async Task<IActionResult> BorrowBook(Guid bookId, [FromBody] BorrowBookRequest request)
        {
            if (request.UserId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            try
            {
                var result = await _bookService.BorrowBookAsync(bookId, request.UserId, request.DueDate);

                if (result == null)
                {
                    var bookDetails = await _bookService.GetBookDetailsByIdAsync(bookId);
                    
                    if (bookDetails == null)
                    {
                        return NotFound(new { message = "Book not found" });
                    }

                    if (bookDetails.AvailableCopies <= 0)
                    {
                        return BadRequest(new { message = "No copies available for borrowing" });
                    }

                    return BadRequest(new { message = "Unable to borrow book. You may already have a pending or active request for this book, or the user does not exist." });
                }

                return Ok(new BorrowBookResponse
                {
                    BorrowingId = result.BorrowingId,
                    AvailableCopies = result.AvailableCopies,
                    Message = "Borrowing request submitted successfully! Waiting for admin approval.",
                    Status = "pending"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Concurrency conflict occurred. Please try again." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while borrowing the book", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request)
        {
            var result = await _bookService.CreateBookAsync(request);
            if (result == null)
                return BadRequest(new { message = "Failed to create book" });
            
            return CreatedAtAction(nameof(GetBookDetails), new { bookId = result.BookId }, result);
        }

        [HttpPut("{bookId}/copies")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBookCopies(Guid bookId, [FromBody] UpdateBookCopiesRequest request)
        {
            var result = await _bookService.UpdateBookCopiesAsync(bookId, request.TotalCopies);
            if (result == null)
                return NotFound(new { message = "Book not found or invalid operation" });
            
            return Ok(result);
        }

        [HttpDelete("{bookId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            var success = await _bookService.DeleteBookAsync(bookId);
            if (!success)
                return NotFound(new { message = "Book not found or has active borrowings" });
            
            return Ok(new { message = "Book deleted successfully" });
        }
    }
}
