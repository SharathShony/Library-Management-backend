using System.Data;
using Microsoft.Data.SqlClient;

namespace Libraray.Api.Helpers.StoredProcedures
{
  public static class RepositoryHelper
    {
   public static async Task<List<TResult>> ExecuteQueryAsync<TInput, TResult>(IConnectionFactory connectionFactory,StoredProcedureParams<TInput> parameters,Func<IDataReader, TResult> resultMapper)
   {
    using var connection = connectionFactory.CreateConnection() as SqlConnection;
    if (connection == null)
    throw new InvalidOperationException("Connection must be a SqlConnection");
    await connection.OpenAsync();
    using var command = new SqlCommand(parameters.StoredProcedureName, connection)
    {
      CommandType = CommandType.StoredProcedure
    };

 // Add parameters
    foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
    {
    var param = new SqlParameter
    {
         ParameterName = name,
         Value = value ?? DBNull.Value,
         Direction = direction
     };    
    if (dbType.HasValue)
     {
        param.DbType = dbType.Value;
     }

    // Handle table-valued parameters (DataTable)
    if (value is DataTable)
    {
        param.SqlDbType = SqlDbType.Structured;
        param.TypeName = typeName ?? throw new InvalidOperationException(
            $"TypeName must be specified for table-valued parameter '{name}'");
    }

    command.Parameters.Add(param);
    }

    var results = new List<TResult>();
     
   using var reader = await command.ExecuteReaderAsync();
   while (await reader.ReadAsync())
   {
    results.Add(resultMapper(reader));
   }

   return results;
  }

        public static async Task<TResult?> ExecuteScalarAsync<TInput, TResult>(IConnectionFactory connectionFactory,StoredProcedureParams<TInput> parameters) where TResult : struct
        {
   using var connection = connectionFactory.CreateConnection() as SqlConnection;
      if (connection == null)
        throw new InvalidOperationException("Connection must be a SqlConnection");
    
   await connection.OpenAsync();

  using var command = new SqlCommand(parameters.StoredProcedureName, connection)
          {
   CommandType = CommandType.StoredProcedure
      };

  foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
  {
        var param = new SqlParameter
          {
 ParameterName = name,
     Value = value ?? DBNull.Value,
          Direction = direction
      };
  
  if (dbType.HasValue)
   {
        param.DbType = dbType.Value;
   }

    // Handle table-valued parameters (DataTable)
    if (value is DataTable)
    {
      param.SqlDbType = SqlDbType.Structured;
        param.TypeName = typeName ?? throw new InvalidOperationException(
 $"TypeName must be specified for table-valued parameter '{name}'");
    }

         command.Parameters.Add(param);
   }

      var result = await command.ExecuteScalarAsync();
       
      if (result == null || result == DBNull.Value)
    return null;

 return (TResult)Convert.ChangeType(result, typeof(TResult));
        }

    public static async Task<int> ExecuteNonQueryAsync<TInput>(IConnectionFactory connectionFactory,StoredProcedureParams<TInput> parameters)
     {
        using var connection = connectionFactory.CreateConnection() as SqlConnection;
        if (connection == null)
             throw new InvalidOperationException("Connection must be a SqlConnection");
    
           await connection.OpenAsync();

          using var command = new SqlCommand(parameters.StoredProcedureName, connection)
 {
           CommandType = CommandType.StoredProcedure
            };    
          foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
             {
            var param = new SqlParameter
            {
                ParameterName = name,
              Value = value ?? DBNull.Value,
                Direction = direction
            };
        
             if (dbType.HasValue)
     {
     param.DbType = dbType.Value;
    }

    // Handle table-valued parameters (DataTable)
    if (value is DataTable)
    {
        param.SqlDbType = SqlDbType.Structured;
        param.TypeName = typeName ?? throw new InvalidOperationException(
   $"TypeName must be specified for table-valued parameter '{name}'");
    }

      command.Parameters.Add(param);
            }

         return await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Execute stored procedure with output parameters
    /// Returns a dictionary of output parameter names and values
        /// </summary>
public static async Task<Dictionary<string, object?>> ExecuteNonQueryWithOutputAsync<TInput>(
        IConnectionFactory connectionFactory,
     StoredProcedureParams<TInput> parameters)
   {
  using var connection = connectionFactory.CreateConnection() as SqlConnection;
     if (connection == null)
  throw new InvalidOperationException("Connection must be a SqlConnection");

      await connection.OpenAsync();

       using var command = new SqlCommand(parameters.StoredProcedureName, connection)
         {
            CommandType = CommandType.StoredProcedure
         };

      // Add parameters and keep track of output parameters
    var outputParams = new List<SqlParameter>();

   foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
            {
             var param = new SqlParameter
                  {
                  ParameterName = name,
                  Value = value ?? DBNull.Value,
                  Direction = direction
                  };

    if (dbType.HasValue)
     {
       param.DbType = dbType.Value;
 
      // Set size for output string parameters
  if (direction == ParameterDirection.Output && dbType.Value == DbType.String)
   {
         param.Size = size ?? -1;  // Use provided size or -1 for MAX
  }
 }

    // Handle table-valued parameters (DataTable)
    if (value is DataTable)
 {
        param.SqlDbType = SqlDbType.Structured;
        param.TypeName = typeName ?? throw new InvalidOperationException(
            $"TypeName must be specified for table-valued parameter '{name}'");
    }

 command.Parameters.Add(param);

  // Track output parameters
    if (direction == ParameterDirection.Output || direction == ParameterDirection.InputOutput)
      {
         outputParams.Add(param);
       }
       }

        // Execute the command
    await command.ExecuteNonQueryAsync();

   // Extract output parameter values
    var outputValues = new Dictionary<string, object?>();
        foreach (var param in outputParams)
    {
   outputValues[param.ParameterName] = param.Value == DBNull.Value ? null : param.Value;
  }

    return outputValues;
        }

        /// <summary>
  /// Execute stored procedure that returns multiple result sets
        /// First result set is mapped to the main DTO, second result set populates a collection property
     /// </summary>
      public static async Task<TMain?> ExecuteMultipleResultSetsAsync<TInput, TMain, TDetail>(
       IConnectionFactory connectionFactory,
  StoredProcedureParams<TInput> parameters,
   Func<IDataReader, TMain> mainMapper,
       Func<IDataReader, TDetail> detailMapper,
       Action<TMain, List<TDetail>> setDetails)
       where TMain : class
        {
   using var connection = connectionFactory.CreateConnection() as SqlConnection;
   if (connection == null)
     throw new InvalidOperationException("Connection must be a SqlConnection");

        await connection.OpenAsync();

       using var command = new SqlCommand(parameters.StoredProcedureName, connection)
     {
      CommandType = CommandType.StoredProcedure
     };

      // Add parameters
     foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
  {
    var param = new SqlParameter
     {
        ParameterName = name,
 Value = value ?? DBNull.Value,
       Direction = direction
 };

       if (dbType.HasValue)
          {
    param.DbType = dbType.Value;
       }

    // Handle table-valued parameters (DataTable)
    if (value is DataTable)
    {
        param.SqlDbType = SqlDbType.Structured;
        param.TypeName = typeName ?? throw new InvalidOperationException(
   $"TypeName must be specified for table-valued parameter '{name}'");
    }

     command.Parameters.Add(param);
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

        // Populate the details collection in the main entity
   if (mainResult != null)
  {
      setDetails(mainResult, details);
          }

  return mainResult;
  }

  /// <summary>
   /// Execute stored procedure with table-valued parameters and output parameters
  /// Used for CreateBook which accepts lists of authors and categories
     /// </summary>
    public static async Task<Dictionary<string, object?>> ExecuteNonQueryWithTableValuedParamsAsync<TInput>(
   IConnectionFactory connectionFactory,
      StoredProcedureParams<TInput> parameters)
  {
 using var connection = connectionFactory.CreateConnection() as SqlConnection;
       if (connection == null)
    throw new InvalidOperationException("Connection must be a SqlConnection");

await connection.OpenAsync();

     using var command = new SqlCommand(parameters.StoredProcedureName, connection)
{
     CommandType = CommandType.StoredProcedure
     };

    // Add parameters and keep track of output parameters
     var outputParams = new List<SqlParameter>();

  foreach (var (name, value, dbType, direction, size, typeName) in parameters.Parameters)
{
 var param = new SqlParameter
     {
       ParameterName = name,
     Value = value ?? DBNull.Value,
     Direction = direction
 };

 if (dbType.HasValue)
       {
        param.DbType = dbType.Value;

   // Set size for output string parameters
     if (direction == ParameterDirection.Output && dbType.Value == DbType.String)
 {
  param.Size = size ?? -1;  // Use provided size or -1 for MAX
      }
    }

   // Handle table-valued parameters (DataTable)
   if (value is DataTable)
     {
    param.SqlDbType = SqlDbType.Structured;
   param.TypeName = typeName ?? throw new InvalidOperationException(
  $"TypeName must be specified for table-valued parameter '{name}'");
}

   command.Parameters.Add(param);

  // Track output parameters
    if (direction == ParameterDirection.Output || direction == ParameterDirection.InputOutput)
    {
     outputParams.Add(param);
      }
}

   // Execute the command
   await command.ExecuteNonQueryAsync();

        // Extract output parameter values
    var outputValues = new Dictionary<string, object?>();
          foreach (var param in outputParams)
{
     outputValues[param.ParameterName] = param.Value == DBNull.Value ? null : param.Value;
   }

   return outputValues;
        }
 }
}
