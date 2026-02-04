using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Libraray.Api.Context;
using Libraray.Api.Entities;
using Libraray.Api.DTO.Users;
using Library_backend.Repositories.Interfaces;
using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.Mappers.UserMappers;

namespace Library_backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LibraryDbContext _context;
        private readonly IConnectionFactory _connectionFactory;

        public UserRepository(LibraryDbContext context, IConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        public async Task<UserAuthDto?> GetByEmailForAuthAsync(string email)
        {
            var parameters = GetByEmailForAuthMapper.Parameters(email);
     var resultMapper = GetByEmailForAuthMapper.ResultMapper();
          var results = await RepositoryHelper.ExecuteQueryAsync<string, UserAuthDto>(
         _connectionFactory, 
       parameters, 
 resultMapper);
      
     return results.FirstOrDefault();
        }

        /// <summary>
        /// CONVERTED TO STORED PROCEDURE
        /// Checks if email exists in database
      /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
  var parameters = EmailExistsMapper.Parameters(email);
   var result = await RepositoryHelper.ExecuteScalarAsync<string, int>(
     _connectionFactory,
          parameters);

          return result.HasValue && result.Value > 0;
   }

        public async Task<bool> UsernameExistsAsync(string username)
        {
      //return await _context.Users
      // .AnyAsync(u => u.Username.ToLower() == username.ToLower());
var parameters = UsernameExistsAsyncMapper.Parameters(username);
var result = await RepositoryHelper.ExecuteScalarAsync<string, int>(_connectionFactory, parameters);
return result.HasValue && result.Value > 0;
        }

    public async Task<bool> AddAsync(User user)
    {
        try
        {
            var parameters = AddUserMapper.Parameters(user);
        var rowsAffected = await RepositoryHelper.ExecuteNonQueryAsync(
            _connectionFactory, 
        parameters);
            
    return rowsAffected > 0;
        }
 catch
        {
            return false;  
        }
    }
    }
}
