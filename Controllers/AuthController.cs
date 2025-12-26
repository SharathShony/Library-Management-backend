using Microsoft.AspNetCore.Mvc;
using Libraray.Api.DTOs.Auth;
using Libraray.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Libraray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);

            if (result.Message == "Invalid email or password")
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            // Validate model state
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Call signup service
            var result = await _authService.SignupAsync(request);

            // Return appropriate status code based on message
            if (result.Message == "Invalid email format")
                return BadRequest(result);

            if (result.Message.Contains("Password must"))
                return BadRequest(result);

            if (result.Message == "Email already exists" || result.Message == "Username already taken")
                return Conflict(result);

            if (result.Message == "Failed to create account")
                return StatusCode(500, result);

            // Success
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
              ?? User.FindFirst("sub")?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value
             ?? User.FindFirst("email")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
         {
         UserId = userId,
      Username = username,
       Email = email,
        Role = role,
                    AllClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }

        [HttpPost("create-admin")]
   public async Task<IActionResult> CreateAdmin([FromBody] Libraray.Api.DTO.Admin.CreateAdminRequest request)
        {
            // Simple secret key check - in production, use environment variables
      const string ADMIN_SECRET = "YourSecretKey123!"; // Change this to a secure value
            
            if (request.SecretKey != ADMIN_SECRET)
   {
      return Unauthorized(new { message = "Invalid secret key" });
    }

        var signupRequest = new SignupRequest
            {
            Username = request.Username,
           Email = request.Email,
           Password = request.Password,
Role = "Admin"
       };

        var result = await _authService.SignupAsync(signupRequest);

     if (result.Message == "Account created successfully")
{
          return Ok(new 
        { 
           message = "Admin account created successfully",
    userId = result.UserId,
     username = result.Username,
          email = result.Email,
     role = "Admin"
         });
            }

     return BadRequest(result);
        }
    }
}
