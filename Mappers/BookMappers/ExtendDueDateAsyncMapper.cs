using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class ExtendDueDateAsyncMapper
    {
        /// <summary>
        /// Maps parameters for usp_ExtendDueDate stored procedure
        /// </summary>
      public static StoredProcedureParams<object> Parameters(Guid borrowingId, int extensionDays)
        {
  var parameters = new StoredProcedureParams<object>("usp_extend_due_date");
       
  // Input parameters
     parameters.AddInputParameter("p_borrowing_id", borrowingId, DbType.Guid);
   parameters.AddInputParameter("p_extension_days", extensionDays, DbType.Int32);
  
    // Output parameters
      parameters.AddOutputParameter("new_due_date", DbType.Date);
          parameters.AddOutputParameter("error_code", DbType.Int32);
parameters.AddOutputParameter("error_message", DbType.String, 255);
            
            return parameters;
      }
    }
}
