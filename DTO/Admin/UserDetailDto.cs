namespace Libraray.Api.DTO.Admin
{
    /// <summary>
    /// DTO for detailed user information with borrowing statistics
 /// </summary>
    public class UserDetailDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
   public DateTime CreatedAt { get; set; }
    public int CurrentBorrowedCount { get; set; }
        public int TotalBorrowedCount { get; set; }
  public int OverdueCount { get; set; }
    }
}
