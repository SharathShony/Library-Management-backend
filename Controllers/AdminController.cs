using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Libraray.Api.Services.Interfaces;
using Libraray.Api.DTO.Admin;
using System.Security.Claims;

namespace Libraray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IAdminService _adminService;

        public AdminController(IBookService bookService, IAdminService adminService)
        {
            _bookService = bookService;
            _adminService = adminService;
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
        public async Task<ActionResult<IEnumerable<UserListItemDto>>> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
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

            var user = await _adminService.GetUserByIdAsync(userId);

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
        public async Task<ActionResult<IEnumerable<UserBorrowedBookDto>>> GetUserBorrowedBooks(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid userId" });
            }

            var borrowedBooks = await _adminService.GetUserBorrowedBooksAsync(userId);

            if (!borrowedBooks.Any())
            {
                // Check if user doesn't exist or just has no borrowed books
                var user = await _adminService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
            }

            return Ok(borrowedBooks);
        }

        /// <summary>
        /// Get all pending borrowing requests for admin review
        /// </summary>
        [HttpGet("borrowing-requests/pending")]
        public async Task<ActionResult<IEnumerable<PendingBorrowingRequestDto>>> GetPendingBorrowingRequests()
        {
            var pendingRequests = await _adminService.GetPendingBorrowingRequestsAsync();
            return Ok(pendingRequests);
        }

        /// <summary>
        /// Admin approves a pending borrowing request
        /// </summary>
        [HttpPost("borrowing-requests/{borrowingId:guid}/approve")]
        public async Task<IActionResult> ApproveBorrowingRequest(Guid borrowingId)
        {
            // Get admin user ID from JWT token
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(adminIdClaim) || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                return Unauthorized(new { message = "Admin not authenticated" });
            }

            var (success, message) = await _adminService.ApproveBorrowingRequestAsync(borrowingId, adminId);

            if (!success)
            {
                if (message.Contains("not found"))
                    return NotFound(new { message });

                return BadRequest(new { message });
            }

            return Ok(new { message, borrowingId });
        }

        /// <summary>
        /// Admin rejects a pending borrowing request with a reason
        /// </summary>
        [HttpPost("borrowing-requests/{borrowingId:guid}/reject")]
        public async Task<IActionResult> RejectBorrowingRequest(Guid borrowingId, [FromBody] RejectBorrowingRequestDto dto)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get admin user ID from JWT token
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(adminIdClaim) || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                return Unauthorized(new { message = "Admin not authenticated" });
            }

            var (success, message) = await _adminService.RejectBorrowingRequestAsync(borrowingId, adminId, dto.Reason);

            if (!success)
            {
                if (message.Contains("not found"))
                    return NotFound(new { message });

                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
    }
}
