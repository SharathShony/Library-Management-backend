using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public class GetCurrentlyBorrowedCountAsyncMapper
    {
        public static StoredProcedureParams<Guid> Parameters(Guid userId) =>
            new StoredProcedureParams<Guid>("usp_get_currently_borrowed_count")
                .AddInputParameter("p_user_id", userId, DbType.Guid);
    }
}
