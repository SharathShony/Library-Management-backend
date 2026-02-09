using System.Data;
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
            new StoredProcedureParams<User>("sp_add_user")
                .AddInputParameter("p_id", user.Id)
                .AddInputParameter("p_username", user.Username)
                .AddInputParameter("p_email", user.Email)
                .AddInputParameter("p_password_hash", user.PasswordHash)
                .AddInputParameter("p_role", user.Role)
                .AddInputParameter("p_created_at", user.CreatedAt, DbType.DateTime2)
                .AddInputParameter("p_updated_at", user.UpdatedAt, DbType.DateTime2);
    }
}
