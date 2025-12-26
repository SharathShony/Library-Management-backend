namespace Libraray.Api.DTO.Admin
{
    /// <summary>
    /// DTO for user list item with borrowing statistics
    /// </summary>
    public class UserListItemDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
     public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
      public int CurrentBorrowedCount { get; set; }
      public int TotalBorrowedCount { get; set; }
    }
}
