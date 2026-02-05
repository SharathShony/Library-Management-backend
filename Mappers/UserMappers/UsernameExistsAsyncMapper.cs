using Libraray.Api.Helpers.StoredProcedures;

namespace Libraray.Api.Mappers.UserMappers
{
    public class UsernameExistsAsyncMapper
    {
        public static StoredProcedureParams<string> Parameters(string username) =>
            new StoredProcedureParams<string>("sp_UsernameExists")
    .AddInputParameter("@UserName", username);
    }
}
