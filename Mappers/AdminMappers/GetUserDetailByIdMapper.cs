using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.DTO.Admin;
using System.Data;

namespace Libraray.Api.Mappers.AdminMappers
{
    public static class GetUserDetailByIdMapper
    {
        /// <summary>
        /// Maps userId parameter for stored procedure
 /// </summary>
  public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
   new StoredProcedureParams<Guid>("sp_get_user_detail_by_id")
       .AddInputParameter("p_user_id", userId, DbType.Guid);

        /// <summary>
        /// Maps database reader to UserDetailDto
        /// </summary>
   public static Func<IDataReader, UserDetailDto> ResultMapper() => reader => new UserDetailDto
  {
     UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
            Username = reader.GetString(reader.GetOrdinal("username")),
      Email = reader.GetString(reader.GetOrdinal("email")),
     Role = reader.GetString(reader.GetOrdinal("role")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
    CurrentBorrowedCount = reader.GetInt32(reader.GetOrdinal("current_borrowed_count")),
            TotalBorrowedCount = reader.GetInt32(reader.GetOrdinal("total_borrowed_count")),
       OverdueCount = reader.GetInt32(reader.GetOrdinal("overdue_count"))
 };
    }
}
