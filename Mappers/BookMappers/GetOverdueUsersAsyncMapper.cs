using Libraray.Api.DTO.Admin;
using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class GetOverdueUsersAsyncMapper
    {
        public static StoredProcedureParams<string> Parameters() =>
            new StoredProcedureParams<string>("dbo.usp_GetOverdueUsers");

        public static Func<IDataReader, OverdueUserDto> ResultMapper() => reader => new OverdueUserDto
        {
            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
            UserName = reader.GetString(reader.GetOrdinal("user_name")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            OverdueCount = reader.GetInt32(reader.GetOrdinal("overdue_count"))
        };
    }
}
