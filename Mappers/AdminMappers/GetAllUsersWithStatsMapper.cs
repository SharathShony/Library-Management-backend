using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.DTO.Admin;
using System.Data;

namespace Libraray.Api.Mappers.AdminMappers
{
  public static class GetAllUsersWithStatsMapper
    {
     /// <summary>
        /// No parameters needed for getting all users
        /// </summary>
      public static StoredProcedureParams<object> Parameters() =>
     new StoredProcedureParams<object>("usp_get_all_users_with_stats"); 

        /// <summary>
        /// Maps database reader to UserListItemDto
        /// </summary>
        public static Func<IDataReader, UserListItemDto> ResultMapper() => reader => new UserListItemDto
        {
    UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
   Username = reader.GetString(reader.GetOrdinal("username")),
    Email = reader.GetString(reader.GetOrdinal("email")),
            Role = reader.GetString(reader.GetOrdinal("role")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
      CurrentBorrowedCount = reader.GetInt32(reader.GetOrdinal("current_borrowed_count")),
         TotalBorrowedCount = reader.GetInt32(reader.GetOrdinal("total_borrowed_count"))
        };
    }
}
