using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Libraray.Api.Helpers.StoredProcedures
{
    public static class RepositoryHelper
    {
        /// <summary>
     /// Builds a PostgreSQL function call SQL string: SELECT * FROM function_name(@param1, @param2, ...)
      /// Strips the "dbo." prefix if present (SQL Server artifact).
        /// </summary>
      private static string BuildFunctionCallSql(string storedProcedureName, IEnumerable<string> parameterNames)
      {
   // Remove "dbo." prefix – PostgreSQL doesn't use schema-qualified names the same way
  var functionName = storedProcedureName.StartsWith("dbo.", StringComparison.OrdinalIgnoreCase)
 ? storedProcedureName.Substring(4)
         : storedProcedureName;

var paramList = string.Join(", ", parameterNames.Select(
    n => n.StartsWith("@") ? n : "@" + n));

            return string.IsNullOrEmpty(paramList)
      ? $"SELECT * FROM {functionName}()"
    : $"SELECT * FROM {functionName}({paramList})";
        }

        /// <summary>
        /// Creates an NpgsqlParameter, stripping the leading '@' for PostgreSQL compatibility.
        /// </summary>
   private static NpgsqlParameter CreateParameter(
          string name, object? value, DbType? dbType, ParameterDirection direction, int? size)
        {
   // When targeting PostgreSQL 'timestamp without time zone' (DbType.DateTime2),
   // Npgsql 6+ rejects DateTime values with Kind=UTC. Convert to Unspecified.
   if (dbType == DbType.DateTime2 && value is DateTime dt && dt.Kind != DateTimeKind.Unspecified)
   {
       value = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
   }

   var param = new NpgsqlParameter
          {
      ParameterName = name,
       Value = value ?? DBNull.Value,
 Direction = direction
          };

      if (dbType.HasValue)
    {
     param.DbType = dbType.Value;

      if (direction == ParameterDirection.Output && dbType.Value == DbType.String)
      {
          param.Size = size ?? -1;
                }
 }

            return param;
        }

   public static async Task<List<TResult>> ExecuteQueryAsync<TInput, TResult>(
   IConnectionFactory connectionFactory,
      StoredProcedureParams<TInput> parameters,
       Func<IDataReader, TResult> resultMapper)
  {
     using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;
          if (connection == null)
       throw new InvalidOperationException("Connection must be an NpgsqlConnection");
  await connection.OpenAsync();

      var sql = BuildFunctionCallSql(
        parameters.StoredProcedureName,
          parameters.Parameters.Select(p => p.Name));

using var command = new NpgsqlCommand(sql, connection)
  {
    CommandType = CommandType.Text
};

       foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
            {
                if (value is DataTable)
    throw new NotSupportedException(
            "PostgreSQL does not support table-valued parameters. Use JSON or arrays instead.");

  command.Parameters.Add(CreateParameter(name, value, dbType, direction, size));
         }

            var results = new List<TResult>();

            using var reader = await command.ExecuteReaderAsync();
 while (await reader.ReadAsync())
            {
     results.Add(resultMapper(reader));
            }

            return results;
        }

        public static async Task<TResult?> ExecuteScalarAsync<TInput, TResult>(
            IConnectionFactory connectionFactory,
            StoredProcedureParams<TInput> parameters) where TResult : struct
        {
      using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;
   if (connection == null)
           throw new InvalidOperationException("Connection must be an NpgsqlConnection");
       await connection.OpenAsync();

     var sql = BuildFunctionCallSql(
   parameters.StoredProcedureName,
       parameters.Parameters.Select(p => p.Name));

         using var command = new NpgsqlCommand(sql, connection)
            {
    CommandType = CommandType.Text
};

            foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
       {
         command.Parameters.Add(CreateParameter(name, value, dbType, direction, size));
            }

     var result = await command.ExecuteScalarAsync();

          if (result == null || result == DBNull.Value)
           return null;

            return (TResult)Convert.ChangeType(result, typeof(TResult));
    }

      public static async Task<int> ExecuteNonQueryAsync<TInput>(
    IConnectionFactory connectionFactory,
            StoredProcedureParams<TInput> parameters)
  {
    using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;
            if (connection == null)
       throw new InvalidOperationException("Connection must be an NpgsqlConnection");
            await connection.OpenAsync();

            var sql = BuildFunctionCallSql(
   parameters.StoredProcedureName,
        parameters.Parameters.Where(p => p.Direction == ParameterDirection.Input).Select(p => p.Name));

          using var command = new NpgsqlCommand(sql, connection)
       {
                CommandType = CommandType.Text
            };

     foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
{
      command.Parameters.Add(CreateParameter(name, value, dbType, direction, size));
   }

            return await command.ExecuteNonQueryAsync();
        }

/// <summary>
        /// Execute function with output parameters.
        /// In PostgreSQL, OUT parameters are returned as columns in the result set.
        /// </summary>
        public static async Task<Dictionary<string, object?>> ExecuteNonQueryWithOutputAsync<TInput>(
 IConnectionFactory connectionFactory,
            StoredProcedureParams<TInput> parameters)
{
       using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;
     if (connection == null)
                throw new InvalidOperationException("Connection must be an NpgsqlConnection");
     await connection.OpenAsync();

      // Only pass input parameters in the function call; OUT params are returned as columns
            var inputParams = parameters.Parameters
 .Where(p => p.Direction == ParameterDirection.Input)
        .ToList();

      var sql = BuildFunctionCallSql(
    parameters.StoredProcedureName,
     inputParams.Select(p => p.Name));

  using var command = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
    };

   foreach (var (name, value, dbType, direction, size, typeName) in inputParams)
  {
        command.Parameters.Add(CreateParameter(name, value, dbType, direction, size));
     }

        var outputValues = new Dictionary<string, object?>();

   // PostgreSQL functions return OUT parameters as columns in the result set
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
     {
      foreach (var (name, _, _, direction, _, _) in parameters.Parameters)
           {
        if (direction == ParameterDirection.Output || direction == ParameterDirection.InputOutput)
   {
     // Strip '@' prefix to match column name
      var columnName = name.StartsWith("@") ? name.Substring(1) : name;
          try
               {
                var ordinal = reader.GetOrdinal(columnName);
          outputValues[name] = reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
            }
                catch (IndexOutOfRangeException)
                {
  outputValues[name] = null;
        }
      }
       }
   }

     return outputValues;
      }

  /// <summary>
        /// Execute function that returns multiple result sets.
   /// Note: PostgreSQL functions don't natively support multiple result sets.
/// This uses SETOF RECORD or multiple OUT parameters with cursors.
        /// </summary>
public static async Task<TMain?> ExecuteMultipleResultSetsAsync<TInput, TMain, TDetail>(
            IConnectionFactory connectionFactory,
       StoredProcedureParams<TInput> parameters,
       Func<IDataReader, TMain> mainMapper,
      Func<IDataReader, TDetail> detailMapper,
   Action<TMain, List<TDetail>> setDetails)
   where TMain : class
     {
            using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;
            if (connection == null)
throw new InvalidOperationException("Connection must be an NpgsqlConnection");
        await connection.OpenAsync();

    var sql = BuildFunctionCallSql(
     parameters.StoredProcedureName,
           parameters.Parameters.Select(p => p.Name));

         using var command = new NpgsqlCommand(sql, connection)
   {
      CommandType = CommandType.Text
            };

          foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
     {
            command.Parameters.Add(CreateParameter(name, value, dbType, direction, size));
     }

      TMain? mainResult = null;
        var details = new List<TDetail>();

            using var reader = await command.ExecuteReaderAsync();

  // First result set - main entity
  if (await reader.ReadAsync())
    {
          mainResult = mainMapper(reader);
    }

        // Second result set - details collection
            if (await reader.NextResultAsync())
            {
      while (await reader.ReadAsync())
        {
          details.Add(detailMapper(reader));
     }
  }

            if (mainResult != null)
        {
        setDetails(mainResult, details);
      }

       return mainResult;
        }

        /// <summary>
        /// Execute function with JSON parameters (PostgreSQL alternative to table-valued params).
    /// </summary>
        public static async Task<Dictionary<string, object?>> ExecuteNonQueryWithJsonParamsAsync<TInput>(
            IConnectionFactory connectionFactory,
            StoredProcedureParams<TInput> parameters)
        {
            using var connection = connectionFactory.CreateConnection() as NpgsqlConnection;
            if (connection == null)
   throw new InvalidOperationException("Connection must be an NpgsqlConnection");
    await connection.OpenAsync();

    // Only pass input parameters in the function call; OUT params are returned as columns
      var inputParams = parameters.Parameters
    .Where(p => p.Direction == ParameterDirection.Input)
                .ToList();

            var sql = BuildFunctionCallSql(
          parameters.StoredProcedureName,
  inputParams.Select(p => p.Name));

            using var command = new NpgsqlCommand(sql, connection)
    {
                CommandType = CommandType.Text
  };

   foreach (var (name, value, dbType, direction, size, typeName) in inputParams)
  {
       command.Parameters.Add(CreateParameter(name, value, dbType, direction, size));
   }

            var outputValues = new Dictionary<string, object?>();

 // PostgreSQL functions return OUT parameters as columns in the result set
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
     foreach (var (name, _, _, direction, _, _) in parameters.Parameters)
     {
         if (direction == ParameterDirection.Output || direction == ParameterDirection.InputOutput)
      {
      var columnName = name.StartsWith("@") ? name.Substring(1) : name;
    try
       {
       var ordinal = reader.GetOrdinal(columnName);
      outputValues[name] = reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
        }
            catch (IndexOutOfRangeException)
         {
      outputValues[name] = null;
             }
     }
       }
     }

       return outputValues;
        }
    }
}
