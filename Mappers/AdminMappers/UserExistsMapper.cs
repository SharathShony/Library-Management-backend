using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.AdminMappers
{
    public static class UserExistsMapper
    {
   /// <summary>
        /// Maps userId parameter for stored procedure
        /// </summary>
   public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
new StoredProcedureParams<Guid>("sp_user_exists")
         .AddInputParameter("p_user_id", userId, DbType.Guid);
    }
}
