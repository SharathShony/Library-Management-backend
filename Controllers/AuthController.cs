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
    }
}
