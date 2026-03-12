using Libraray.Api.Helpers.StoredProcedures;
using Libraray.Api.Entities;
using System.Data;

namespace Libraray.Api.Mappers.AdminMappers
{
    public static class UpdateBorrowingMapper
 {
   /// <summary>
        /// Maps Borrowing entity parameters for stored procedure
   /// </summary>
        public static StoredProcedureParams<Borrowing> Parameters(Borrowing borrowing) =>
   new StoredProcedureParams<Borrowing>("sp_update_borrowing")
    .AddInputParameter("p_borrowing_id", borrowing.Id, DbType.Guid)
    .AddInputParameter("p_status", borrowing.Status, DbType.String)
   .AddInputParameter("p_approved_by", borrowing.ApprovedBy, DbType.Guid)
       .AddInputParameter("p_approved_at", borrowing.ApprovedAt, DbType.DateTime2)
        .AddInputParameter("p_rejection_reason", borrowing.RejectionReason, DbType.String);
    }
}
