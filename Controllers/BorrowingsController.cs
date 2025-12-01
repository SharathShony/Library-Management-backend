using Microsoft.AspNetCore.Mvc;
using Libraray.Api.Services.Interfaces;

namespace Libraray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}