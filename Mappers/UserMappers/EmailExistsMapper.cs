using Libraray.Api.Helpers.StoredProcedures;

namespace Libraray.Api.Mappers.UserMappers
{
    public static class EmailExistsMapper
    {
        /// <summary>
        /// Maps input parameter to stored procedure parameters
        /// </summary>
        public static StoredProcedureParams<string> Parameters(string email) =>
            new StoredProcedureParams<string>("sp_EmailExists")
    .AddInputParameter("@email", email);
    }
}
