using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Libraray.Api.Services.Interfaces;
using Libraray.Api.DTO.Admin;

namespace Libraray.Api.Controllers
{
 [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
  private readonly IBookService _bookService;

        public AdminController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("overdue-users")]
     public async Task<ActionResult<IEnumerable<OverdueUserDto>>> GetOverdueUsers()
        {
        var overdueUsers = await _bookService.GetOverdueUsersAsync();
            return Ok(overdueUsers);
        }

   [HttpGet("overdue-books/{userId}")]
        public async Task<ActionResult<UserOverdueBooksDto>> GetUserOverdueBooks(Guid userId)
        {
  if (userId == Guid.Empty)
  {
    return BadRequest(new { message = "Invalid userId" });
            }

            var result = await _bookService.GetUserOverdueBooksAsync(userId);

          if (result == null)
       {
     return NotFound(new { message = "User not found" });
         }

            return Ok(result);
  }
    }
}
