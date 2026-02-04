using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.DTO.Users;
using System.Data;

namespace Libraray.Api.Mappers.UserMappers
{
    public static class GetByEmailForAuthMapper
    {
        /// <summary>
        /// Maps input email parameter to stored procedure parameters
        /// </summary>
        public static StoredProcedureParams<string> Parameters(string email) =>
            new StoredProcedureParams<string>("sp_GetByEmailForAuth")
        .AddInputParameter("@email", email);

        /// <summary>
        /// Maps database reader to UserAuthDto
        /// </summary>
        public static Func<IDataReader, UserAuthDto> ResultMapper() => reader => new UserAuthDto
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            Username = reader.GetString(reader.GetOrdinal("username")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
            Role = reader.GetString(reader.GetOrdinal("role"))
        };
    }
}
