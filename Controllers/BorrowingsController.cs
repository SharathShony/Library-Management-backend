using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Libraray.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints in this controller
    public class BorrowingsController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BorrowingsController(IBookService bookService)
        {
            _bookService = bookService;
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
    }
}