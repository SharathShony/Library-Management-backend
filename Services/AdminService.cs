using Libraray.Api.DTO.Admin;
using Libraray.Api.Repositories.Interfaces;
using Libraray.Api.Services.Interfaces;

namespace Libraray.Api.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

      public AdminService(IAdminRepository adminRepository)
        {
   _adminRepository = adminRepository;
        }

        public async Task<IEnumerable<UserListItemDto>> GetAllUsersAsync()
{
            return await _adminRepository.GetAllUsersWithStatsAsync();
        }

        public async Task<UserDetailDto?> GetUserByIdAsync(Guid userId)
        {
      if (userId == Guid.Empty)
  return null;

return await _adminRepository.GetUserDetailByIdAsync(userId);
        }

        public async Task<IEnumerable<UserBorrowedBookDto>> GetUserBorrowedBooksAsync(Guid userId)
        {
          if (userId == Guid.Empty)
           return Enumerable.Empty<UserBorrowedBookDto>();

   var userExists = await _adminRepository.UserExistsAsync(userId);
 if (!userExists)
      return Enumerable.Empty<UserBorrowedBookDto>();

            return await _adminRepository.GetUserBorrowedBooksAsync(userId);
        }

        public async Task<IEnumerable<PendingBorrowingRequestDto>> GetPendingBorrowingRequestsAsync()
        {
            return await _adminRepository.GetPendingBorrowingRequestsAsync();
        }

      public async Task<(bool Success, string Message)> ApproveBorrowingRequestAsync(Guid borrowingId, Guid adminId)
   {
            var borrowing = await _adminRepository.GetBorrowingByIdWithDetailsAsync(borrowingId);

   if (borrowing == null)
   return (false, "Borrowing request not found");

            if (borrowing.Status != "pending")
    return (false, $"Request already {borrowing.Status}");

            if (borrowing.Book.AvailableCopies <= 0)
     return (false, "Book is no longer available");

            // Approve the request
        borrowing.Status = "approved";
    borrowing.ApprovedBy = adminId;
   borrowing.ApprovedAt = DateTime.UtcNow;

        var borrowingUpdated = await _adminRepository.UpdateBorrowingAsync(borrowing);
            if (!borrowingUpdated)
    return (false, "Failed to update borrowing request");

     // Decrement available copies
        var copiesUpdated = await _adminRepository.UpdateBookAvailableCopiesAsync(borrowing.BookId, -1);
            if (!copiesUpdated)
        return (false, "Failed to update book availability");

            return (true, $"Borrowing request for '{borrowing.Book.Title}' approved successfully");
        }

      public async Task<(bool Success, string Message)> RejectBorrowingRequestAsync(Guid borrowingId, Guid adminId, string reason)
        {
         if (string.IsNullOrWhiteSpace(reason))
                return (false, "Rejection reason is required");

         var borrowing = await _adminRepository.GetBorrowingByIdWithDetailsAsync(borrowingId);

         if (borrowing == null)
            return (false, "Borrowing request not found");

     if (borrowing.Status != "pending")
           return (false, $"Request already {borrowing.Status}");

            // Reject the request
 borrowing.Status = "rejected";
       borrowing.ApprovedBy = adminId;
            borrowing.ApprovedAt = DateTime.UtcNow;
            borrowing.RejectionReason = reason;

        var updated = await _adminRepository.UpdateBorrowingAsync(borrowing);
            if (!updated)
  return (false, "Failed to update borrowing request");

      return (true, $"Borrowing request for '{borrowing.Book.Title}' rejected");
        }
    }
}
