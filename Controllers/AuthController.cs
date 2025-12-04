using Microsoft.AspNetCore.Mvc;
using Libraray.Api.DTOs.Auth;
using Libraray.Api.Services.Interfaces;

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
    }
}
