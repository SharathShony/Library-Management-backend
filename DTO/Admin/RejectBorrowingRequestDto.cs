using System.ComponentModel.DataAnnotations;

namespace Libraray.Api.DTO.Admin
{
  /// <summary>
    /// DTO for rejecting a borrowing request
    /// </summary>
    public class RejectBorrowingRequestDto
    {
        [Required(ErrorMessage = "Rejection reason is required")]
        [StringLength(500, ErrorMessage = "Reason must not exceed 500 characters")]
        public string Reason { get; set; } = null!;
    }
}
