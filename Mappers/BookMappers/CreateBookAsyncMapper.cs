using Libraray.Api.DTO.Books;
using Libraray.Api.Helpers.StoredProcedures;
using System.Data;

namespace Libraray.Api.Mappers.BookMappers
{
    public static class CreateBookAsyncMapper
    {
        /// <summary>
        /// Maps CreateBookRequest to stored procedure parameters with array parameters
        /// </summary>
        public static StoredProcedureParams<object> Parameters(CreateBookRequest request)
        {
            var parameters = new StoredProcedureParams<object>("usp_create_book");
 
            // Input parameters
            parameters.AddInputParameter("p_title", request.Title, DbType.String);
            parameters.AddInputParameter("p_subtitle", request.Subtitle, DbType.String);
            parameters.AddInputParameter("p_isbn", request.Isbn, DbType.String);
            parameters.AddInputParameter("p_summary", request.Summary, DbType.String);
            parameters.AddInputParameter("p_publisher", request.Publisher, DbType.String);
            parameters.AddInputParameter("p_publication_date", request.PublicationDate, DbType.Date);
            parameters.AddInputParameter("p_total_copies", request.TotalCopies, DbType.Int32);
        
            // PostgreSQL arrays instead of table-valued parameters
            parameters.AddInputParameter("p_authors", request.Authors?.ToArray() ?? Array.Empty<string>());
            parameters.AddInputParameter("p_categories", request.Categories?.ToArray() ?? Array.Empty<string>());
      
            // Output parameters
            parameters.AddOutputParameter("new_book_id", DbType.Guid);
            parameters.AddOutputParameter("error_code", DbType.Int32);
            parameters.AddOutputParameter("error_message", DbType.String, 255);
    
            return parameters;
        }
    }
}
