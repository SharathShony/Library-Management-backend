using Microsoft.AspNetCore.Mvc;
using Libraray.Api.DTO.Books;
using Libraray.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("catalog")]
        public async Task<IActionResult> GetCatalog()
        {
            var books = await _bookService.GetCatalogAsync(); // ← Fixed: Use GetCatalogAsync
            return Ok(books);
        }

        [HttpGet("{bookId}/details")]
        public async Task<IActionResult> GetBookDetails(Guid bookId)
        {
            var book = await _bookService.GetBookDetailsByIdAsync(bookId);
            
            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            return Ok(book);
        }

        [HttpPost("{bookId}/borrow")]
        public async Task<IActionResult> BorrowBook(Guid bookId, [FromBody] BorrowBookRequest request)
        {
            // Validate request
            if (request.UserId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            try
            {
                var result = await _bookService.BorrowBookAsync(bookId, request.UserId, request.DueDate);

                if (result == null)
                {
                    // Check specific error conditions
                    var bookDetails = await _bookService.GetBookDetailsByIdAsync(bookId);
                    
                    if (bookDetails == null)
                    {
                        return NotFound(new { message = "Book not found" });
                    }

                    if (bookDetails.AvailableCopies <= 0)
                    {
                        return BadRequest(new { message = "No copies available for borrowing" });
                    }

                    return BadRequest(new { message = "Unable to borrow book. User may not exist." });
                }

                return Ok(result);
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
    }
}
