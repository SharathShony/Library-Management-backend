using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Libraray.Api.DTOs.Auth;
using Libraray.Api.Entities;
using Libraray.Api.Services.Interfaces;
using Library_backend.Repositories.Interfaces;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Libraray.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
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

            // 2. Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponse
                {
                    Message = "Invalid email or password"
                };
            }

            // 3. Generate JWT token
            var token = _jwtTokenService.GenerateToken(user);

            // 4. Manual Mapping → Entity → DTO
            return new LoginResponse
            {
                Message = "Login successful",
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        public async Task<SignupResponse> SignupAsync(SignupRequest request)
        {
            try
            {
                // Normalize email to lowercase first
                var normalizedEmail = request.Email.Trim().ToLower();
                
                if (!IsValidEmail(normalizedEmail))
                {
                    return new SignupResponse
                    {
                        Message = "Invalid email format"
                    };
                }

                var passwordValidation = ValidatePassword(request.Password);
                if (!passwordValidation.IsValid)
                {
                    return new SignupResponse
                    {
                        Message = passwordValidation.ErrorMessage
                    };
                }

                // Check with normalized email
                if (await _userRepository.EmailExistsAsync(normalizedEmail))
                {
                    return new SignupResponse
                    {
                        Message = "Email already exists"
                    };
                }

                if (await _userRepository.UsernameExistsAsync(request.Username))
                {
                    return new SignupResponse
                    {
                        Message = "Username already taken"
                    };
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = normalizedEmail, // Use normalized email
                    PasswordHash = hashedPassword,
                    Role = string.IsNullOrWhiteSpace(request.Role) ? "Reader" : request.Role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.AddAsync(newUser);

                return new SignupResponse
                {
                    Message = "Account created successfully",
                    UserId = createdUser.Id,
                    Username = createdUser.Username,
                    Email = createdUser.Email
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("UQ__USERS__AB6E6164") == true)
            {
                // Catch unique constraint violation from database
                return new SignupResponse
                {
                    Message = "Email already exists"
                };
            }
            catch (Exception)
            {
                // Log exception here in production
                return new SignupResponse
                {
                    Message = "Failed to create account"
                };
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // RFC 5322 email validation pattern
                var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private (bool IsValid, string ErrorMessage) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password is required");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long");

            if (password.Length > 100)
                return (false, "Password must not exceed 100 characters");

            // Check for at least one uppercase letter
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return (false, "Password must contain at least one uppercase letter");

            // Check for at least one lowercase letter
            if (!Regex.IsMatch(password, @"[a-z]"))
                return (false, "Password must contain at least one lowercase letter");

            // Check for at least one digit
            if (!Regex.IsMatch(password, @"[0-9]"))
                return (false, "Password must contain at least one number");

            // Check for at least one special character
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
                return (false, "Password must contain at least one special character");

            return (true, string.Empty);
        }
    }
}
