using Libraray.Api.DTO.Users;
using Libraray.Api.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library_backend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // Get user by email for authentication - returns DTO with PasswordHash for verification
        Task<UserAuthDto?> GetByEmailForAuthAsync(string email);

        // Add a new user - returns true if successful, false if failed
        Task<bool> AddAsync(User user);


        // Check if email already exists (registration)
        Task<bool> EmailExistsAsync(string email);

        // Check if username already exists (registration)
        Task<bool> UsernameExistsAsync(string username);
    }
}
