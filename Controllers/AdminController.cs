using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Libraray.Api.Services.Interfaces;
using Libraray.Api.DTO.Admin;
using Libraray.Api.Context;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly LibraryDbContext _context;

        public AdminController(IBookService bookService, LibraryDbContext context)
        {
            _bookService = bookService;
            _context = context;
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

        /// <summary>
        /// Get all users with their borrowing statistics
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<List<UserListItemDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserListItemDto
                {
                    UserId = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    CurrentBorrowedCount = _context.Borrowings
                        .Count(b => b.UserId == u.Id && b.ReturnDate == null),
                    TotalBorrowedCount = _context.Borrowings
                        .Count(b => b.UserId == u.Id)
                })
                .OrderBy(u => u.Username)
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Get detailed information about a specific user
        /// </summary>
        [HttpGet("users/{userId:guid}")]
        public async Task<ActionResult<UserDetailDto>> GetUserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid userId" });
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDetailDto
                {
                    UserId = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    CurrentBorrowedCount = _context.Borrowings
                        .Count(b => b.UserId == u.Id && b.ReturnDate == null),
                    TotalBorrowedCount = _context.Borrowings
                        .Count(b => b.UserId == u.Id),
                    OverdueCount = _context.Borrowings
                        .Count(b => b.UserId == u.Id &&
                                     b.ReturnDate == null &&
                                     b.DueDate.HasValue &&
                                     b.DueDate.Value < today)
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Get all books borrowed by a specific user (both current and history)
        /// </summary>
        [HttpGet("users/{userId:guid}/borrowed-books")]
        public async Task<ActionResult<List<UserBorrowedBookDto>>> GetUserBorrowedBooks(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid userId" });
            }

            // Verify user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound(new { message = "User not found" });
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var borrowedBooks = await _context.Borrowings
                .Where(b => b.UserId == userId)
                .Include(b => b.Book)
                .OrderByDescending(b => b.BorrowDate)
                .Select(b => new UserBorrowedBookDto
                {
                    BorrowingId = b.Id,
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    Isbn = b.Book.Isbn,
                    BorrowedDate = b.BorrowDate.ToDateTime(TimeOnly.MinValue),
                    DueDate = b.DueDate.HasValue ? b.DueDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                    ReturnedDate = b.ReturnDate.HasValue ? b.ReturnDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                    Status = b.ReturnDate != null ? "Returned" :
                            (b.DueDate.HasValue && b.DueDate.Value < today ? "Overdue" : "Borrowed")
                })
                .ToListAsync();

            return Ok(borrowedBooks);
        }
    }
}
