namespace Libraray.Api.DTO.Admin
{
    /// <summary>
    /// DTO for pending borrowing request details
    /// </summary>
  public class PendingBorrowingRequestDto
    {
        public Guid BorrowingId { get; set; }
        public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
      public string BookTitle { get; set; } = null!;
    public Guid BookId { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}
