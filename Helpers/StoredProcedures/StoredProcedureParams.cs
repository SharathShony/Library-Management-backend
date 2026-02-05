using System.Data;

namespace Libraray.Api.Helpers.StoredProcedures
{
    public class StoredProcedureParams<T>
    {
    public string StoredProcedureName { get; }
    public List<(string Name, object? Value, DbType? DbType, ParameterDirection Direction, int? Size, string? TypeName)> Parameters { get; }

    public StoredProcedureParams(string storedProcedureName)
    {
          StoredProcedureName = storedProcedureName;
          Parameters = new List<(string, object?, DbType?, ParameterDirection, int?, string?)>();
    }

    public StoredProcedureParams<T> AddInputParameter(string name, object? value, DbType? dbType = null, string? typeName = null)
    {
        Parameters.Add((name, value, dbType, ParameterDirection.Input, null, typeName));
        return this;
    }

        public StoredProcedureParams<T> AddOutputParameter(string name, DbType dbType, int? size = null)
        {
            Parameters.Add((name, null, dbType, ParameterDirection.Output, size, null));
            return this;
        }

        public StoredProcedureParams<T> AddInputOutputParameter(string name, object? value, DbType dbType, int? size = null)
        {
        Parameters.Add((name, value, dbType, ParameterDirection.InputOutput, size, null));
        return this;
        }
    }
}
