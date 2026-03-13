using Libraray.Api.DTO.Admin;

namespace Libraray.Api.Services.Interfaces
{
    public interface IAdminService
    {
// User management
     Task<IEnumerable<UserListItemDto>> GetAllUsersAsync();
        Task<UserDetailDto?> GetUserByIdAsync(Guid userId);
   Task<IEnumerable<UserBorrowedBookDto>> GetUserBorrowedBooksAsync(Guid userId);
  
// Borrowing request management
        Task<IEnumerable<PendingBorrowingRequestDto>> GetPendingBorrowingRequestsAsync();
Task<(bool Success, string Message)> ApproveBorrowingRequestAsync(Guid borrowingId, Guid adminId);
        Task<(bool Success, string Message)> RejectBorrowingRequestAsync(Guid borrowingId, Guid adminId, string reason);
    }
}
