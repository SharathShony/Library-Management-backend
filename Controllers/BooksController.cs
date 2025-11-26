using Microsoft.AspNetCore.Mvc;
using Libraray.Api.Services.Interfaces;

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
            var books = await _bookService.GetCatalogAsync();
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
    }
}
