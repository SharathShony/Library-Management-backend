using System.Data;
using Libraray.Api.DTO.Books;
using Libraray.Api.Helpers.StoredProcedures;

namespace Libraray.Api.Mappers.CategoryMappers
{
    public static class GetAllCategoriesAsyncMapper
    {
   
        public static StoredProcedureParams<string> Parameters()
        {
         return new StoredProcedureParams<string>("usp_get_all_categories");
        }

        public static Func<IDataReader, CategoryDto> ResultMapper(){
            return reader => new CategoryDto
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name"))
            };
        }
    }
}
