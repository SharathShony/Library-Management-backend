using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.AdminMappers
{
    public static class UpdateBookAvailableCopiesMapper
    {
  /// <summary>
   /// Maps parameters for updating book available copies
    /// </summary>
        public static StoredProcedureParams<object> Parameters(Guid bookId, int copiesChange) =>
new StoredProcedureParams<object>("sp_update_book_available_copies")
       .AddInputParameter("p_book_id", bookId, DbType.Guid)
       .AddInputParameter("p_copies_change", copiesChange, DbType.Int32);
 }
}
