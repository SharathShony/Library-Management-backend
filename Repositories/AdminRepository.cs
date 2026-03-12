using Libraray.Api.Context;
using Libraray.Api.DTO.Admin;
using Libraray.Api.Entities;
using Libraray.Api.Repositories.Interfaces;
using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.Mappers.AdminMappers;

namespace Libraray.Api.Repositories
{
    public class AdminRepository : IAdminRepository
    {
private readonly IConnectionFactory _connectionFactory;

        public AdminRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
}

        public async Task<IEnumerable<UserListItemDto>> GetAllUsersWithStatsAsync()
        {
            var parameters = GetAllUsersWithStatsMapper.Parameters();
            var resultMapper = GetAllUsersWithStatsMapper.ResultMapper();
   var results = await RepositoryHelper.ExecuteQueryAsync<object, UserListItemDto>(
       _connectionFactory,
      parameters,
        resultMapper);

            return results;
        }

        public async Task<UserDetailDto?> GetUserDetailByIdAsync(Guid userId)
   {
            var parameters = GetUserDetailByIdMapper.Parameters(userId);
     var resultMapper = GetUserDetailByIdMapper.ResultMapper();
  var results = await RepositoryHelper.ExecuteQueryAsync<Guid, UserDetailDto>(
       _connectionFactory,
     parameters,
                resultMapper);

            return results.FirstOrDefault();
        }

        public async Task<IEnumerable<UserBorrowedBookDto>> GetUserBorrowedBooksAsync(Guid userId)
    {
            var parameters = GetUserBorrowedBooksMapper.Parameters(userId);
         var resultMapper = GetUserBorrowedBooksMapper.ResultMapper();
 var results = await RepositoryHelper.ExecuteQueryAsync<Guid, UserBorrowedBookDto>(
          _connectionFactory,
       parameters,
       resultMapper);

          return results;
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
       var parameters = UserExistsMapper.Parameters(userId);
            var result = await RepositoryHelper.ExecuteScalarAsync<Guid, int>(
             _connectionFactory,
          parameters);

  return result.HasValue && result.Value > 0;
        }

        public async Task<IEnumerable<PendingBorrowingRequestDto>> GetPendingBorrowingRequestsAsync()
  {
            var parameters = GetPendingBorrowingRequestsMapper.Parameters();
            var resultMapper = GetPendingBorrowingRequestsMapper.ResultMapper();
 var results = await RepositoryHelper.ExecuteQueryAsync<object, PendingBorrowingRequestDto>(
        _connectionFactory,
  parameters,
      resultMapper);

    return results;
      }

   public async Task<Borrowing?> GetBorrowingByIdWithDetailsAsync(Guid borrowingId)
        {
          var parameters = GetBorrowingByIdWithDetailsMapper.Parameters(borrowingId);
            var resultMapper = GetBorrowingByIdWithDetailsMapper.ResultMapper();
     var results = await RepositoryHelper.ExecuteQueryAsync<Guid, BorrowingWithDetails>(
    _connectionFactory,
      parameters,
       resultMapper);

   return results.FirstOrDefault()?.ToBorrowing();
        }

        public async Task<bool> UpdateBorrowingAsync(Borrowing borrowing)
        {
            try
     {
     var parameters = UpdateBorrowingMapper.Parameters(borrowing);
           var result = await RepositoryHelper.ExecuteScalarAsync<Borrowing, int>(
          _connectionFactory,
  parameters);

     return result.HasValue && result.Value > 0;
            }
        catch
          {
  return false;
     }
        }

      public async Task<bool> UpdateBookAvailableCopiesAsync(Guid bookId, int copiesChange)
        {
try
    {
                var parameters = UpdateBookAvailableCopiesMapper.Parameters(bookId, copiesChange);
      var result = await RepositoryHelper.ExecuteScalarAsync<object, int>(
          _connectionFactory,
  parameters);

   return result.HasValue && result.Value > 0;
       }
         catch
            {
     return false;
            }
    }
    }
}
