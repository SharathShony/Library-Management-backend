using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public class GetCurrentlyBorrowedCountAsyncMapper
    {
        public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
            new StoredProcedureParams<Guid>("dbo.usp_GetCurrentlyBorrowedCount")
                .AddInputParameter("@user_id", userId, DbType.Guid);
    }
}
