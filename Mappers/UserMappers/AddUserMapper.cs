using Libraray.Api.Entities;
using Libraray.Api.Helpers.StoredProcedures;

namespace Libraray.Api.Mappers.UserMappers
{
    public static class AddUserMapper
    {
        /// <summary>
        /// Maps User entity to stored procedure parameters
        /// </summary>
        public static StoredProcedureParams<User> Parameters(User user) =>
            new StoredProcedureParams<User>("dbo.usp_AddUser")
                .AddInputParameter("@id", user.Id)
                .AddInputParameter("@username", user.Username)
                .AddInputParameter("@email", user.Email)
                .AddInputParameter("@password_hash", user.PasswordHash)
                .AddInputParameter("@role", user.Role)
                .AddInputParameter("@created_at", user.CreatedAt)
                .AddInputParameter("@updated_at", user.UpdatedAt);
    }
}
