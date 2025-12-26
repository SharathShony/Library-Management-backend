namespace Libraray.Api.DTO.Users
{
    /// <summary>
    /// DTO for authentication - contains password hash for verification only
    /// Should only be used within authentication context
    /// </summary>
    public class UserAuthDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;  // Only for password verification
        public string Role { get; set; } = null!;
    }
}
