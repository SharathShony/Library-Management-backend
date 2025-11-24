using Libraray.Api.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library_backend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // Get all users
        Task<IEnumerable<User>> GetAllAsync();

        // Get specific user by ID
        Task<User> GetByIdAsync(int id);

        // Get user by email (used for login)
        Task<User> GetByEmailAsync(string email);

        // Add a new user
        Task<User> AddAsync(User user);

        // Update user details
        Task<User> UpdateAsync(User user);

        // Delete user
        Task<bool> DeleteAsync(int id);

        // Check if email already exists (registration)
        Task<bool> EmailExistsAsync(string email);
    }
}
