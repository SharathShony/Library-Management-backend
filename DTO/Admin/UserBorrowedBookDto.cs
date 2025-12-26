namespace Libraray.Api.DTO.Admin
{
    /// <summary>
    /// DTO for user's borrowed book information
    /// </summary>
    public class UserBorrowedBookDto
    {
    public Guid BorrowingId { get; set; }
    public Guid BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string? Isbn { get; set; }
   public DateTime BorrowedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
   public string Status { get; set; } = null!;
    }
}
