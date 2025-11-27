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
    }
}