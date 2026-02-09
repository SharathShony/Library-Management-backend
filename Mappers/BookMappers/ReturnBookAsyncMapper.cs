using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class ReturnBookAsyncMapper
    {
  public static StoredProcedureParams<Guid> Parameters(Guid borrowingId)
        {
   var parameters = new StoredProcedureParams<Guid>("usp_return_book");
       
         // Input parameter
     parameters.AddInputParameter("p_borrowing_id", borrowingId, DbType.Guid);
      
       // Output parameters
       parameters.AddOutputParameter("book_id", DbType.Guid);
       parameters.AddOutputParameter("return_date", DbType.Date);
 parameters.AddOutputParameter("available_copies", DbType.Int32);
parameters.AddOutputParameter("error_code", DbType.Int32);
            parameters.AddOutputParameter("error_message", DbType.String, 255);
   
  return parameters;
        }
    }
}
