using System.ComponentModel.DataAnnotations;

namespace Libraray.Api.DTOs.Auth
{
    public class SignupRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150, ErrorMessage = "Email must not exceed 150 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string Password { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Role must not exceed 50 characters")]
        public string? Role { get; set; } // Optional, defaults to "Reader"
    }
}
