namespace Libraray.Api.DTOs.Auth
{
    public class SignupResponse
    {
        public string Message { get; set; } = null!;
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
 }
}
