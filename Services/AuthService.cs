using System.Threading.Tasks;
using Libraray.Api.DTOs.Auth;
using Libraray.Api.Entities;
using Libraray.Api.Services.Interfaces;
using Library_backend.Repositories.Interfaces;

namespace Libraray.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // 1. Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return new LoginResponse
                {
                    Message = "Invalid email or password"
                };
            }

            // 2. Plain password comparison (TEMPORARY)
            if (user.PasswordHash != request.Password)
            {
                return new LoginResponse
                {
                    Message = "Invalid email or password"
                };
            }

            // 3. Manual Mapping → Entity → DTO
            return new LoginResponse
            {
                Message = "Login successful",
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = null // JWT later
            };
        }
    }
}
