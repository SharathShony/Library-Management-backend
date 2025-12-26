namespace Libraray.Api.DTO.Users
{
    /// <summary>
    /// DTO for JWT token generation - contains only claims needed for JWT
    /// Does NOT contain password hash - follows principle of least privilege
    /// </summary>
    public class UserClaimsDto
    {
        public Guid Id { get; set; }
  public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
   public string Role { get; set; } = null!;
        // ? NO PasswordHash - JWT service doesn't need it
        // ? NO Borrowings - JWT service doesn't need it
    }
}
