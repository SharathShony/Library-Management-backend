using Libraray.Api.DTO.Books;
using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class CreateBookAsyncMapper
    {
        /// <summary>
        /// Maps CreateBookRequest to stored procedure parameters with table-valued parameters
        /// </summary>
        public static StoredProcedureParams<object> Parameters(CreateBookRequest request)
        {
            var parameters = new StoredProcedureParams<object>("dbo.usp_CreateBook");
 
            // Input parameters
            parameters.AddInputParameter("@title", request.Title, DbType.String);
            parameters.AddInputParameter("@subtitle", request.Subtitle, DbType.String);
            parameters.AddInputParameter("@isbn", request.Isbn, DbType.String);
            parameters.AddInputParameter("@summary", request.Summary, DbType.String);
            parameters.AddInputParameter("@publisher", request.Publisher, DbType.String);
            parameters.AddInputParameter("@publication_date", request.PublicationDate, DbType.Date);
            parameters.AddInputParameter("@total_copies", request.TotalCopies, DbType.Int32);
        
       // Table-valued parameters for authors
            var authorsTable = new DataTable();
            authorsTable.Columns.Add("Value", typeof(string));
            if (request.Authors != null)
            {
                foreach (var author in request.Authors)
                {
                    authorsTable.Rows.Add(author);
                }
            }
            parameters.AddInputParameter("@authors", authorsTable, null, "dbo.StringListTableType");
   
  // Table-valued parameters for categories
            var categoriesTable = new DataTable();
            categoriesTable.Columns.Add("Value", typeof(string));
            if (request.Categories != null)
            {
                foreach (var category in request.Categories)
                {
                    categoriesTable.Rows.Add(category);
                }
            }
        parameters.AddInputParameter("@categories", categoriesTable, null, "dbo.StringListTableType");
      
      // Output parameters
        parameters.AddOutputParameter("@new_book_id", DbType.Guid);
        parameters.AddOutputParameter("@error_code", DbType.Int32);
        parameters.AddOutputParameter("@error_message", DbType.String, 255);
    
        return parameters;
        }
    }
}
