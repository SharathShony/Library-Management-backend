using Libraray.Api.DTOs.Auth;
using Libraray.Api.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library_backend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // Get user by email (used for login)
        Task<User> GetByEmailAsync(string email);

        // Add a new user - returns true if successful, false if failed
        Task<bool> AddAsync(User user);


        // Check if email already exists (registration)
        Task<bool> EmailExistsAsync(string email);

        // Check if username already exists (registration)
        Task<bool> UsernameExistsAsync(string username);
    }
}
