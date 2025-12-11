using System.ComponentModel.DataAnnotations;

namespace Libraray.Api.DTO.Admin
{
    public class CreateAdminRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Secret key is required for admin creation")]
public string SecretKey { get; set; } = null!;
    }
}
