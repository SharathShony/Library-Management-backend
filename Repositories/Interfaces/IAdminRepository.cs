using Libraray.Api.DTO.Admin;
using Libraray.Api.Entities;

namespace Libraray.Api.Repositories.Interfaces
{
    public interface IAdminRepository
    {
    // User queries
        Task<IEnumerable<UserListItemDto>> GetAllUsersWithStatsAsync();
        Task<UserDetailDto?> GetUserDetailByIdAsync(Guid userId);
        Task<IEnumerable<UserBorrowedBookDto>> GetUserBorrowedBooksAsync(Guid userId);
        Task<bool> UserExistsAsync(Guid userId);

        // Borrowing request queries
        Task<IEnumerable<PendingBorrowingRequestDto>> GetPendingBorrowingRequestsAsync();
        Task<Borrowing?> GetBorrowingByIdWithDetailsAsync(Guid borrowingId);
        Task<bool> UpdateBorrowingAsync(Borrowing borrowing);
        Task<bool> UpdateBookAvailableCopiesAsync(Guid bookId, int copiesChange);
    }
}
