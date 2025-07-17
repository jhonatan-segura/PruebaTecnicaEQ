using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DocProcessing.API.Entities;
using DocProcessing.API.Dtos;

namespace DocProcessing.API.Controllers
{

   [ApiController]
   [Route("LogProcess")]
   public class LogProcessController(IConfiguration config) : ControllerBase
   {
      private readonly IConfiguration _config = config;

      /// <summary>
      /// Obtiene  los registros de la tabla LogProcess (utilizado en el frontend).
      /// </summary>
      /// <param name="id"></param>
      /// <returns>Lista de registros de la tabla LogProcess</returns>
      [HttpGet]
      public async Task<ActionResult<IEnumerable<LogProcess>>> Get()
      {
         var logProcesses = new List<LogProcess>();

         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("GetLogProcesses", connection);
         command.CommandType = CommandType.StoredProcedure;

         using var reader = await command.ExecuteReaderAsync();
         while (await reader.ReadAsync())
         {
            logProcesses.Add(new()
            {
               Id = reader.GetInt32(reader.GetOrdinal("Id")),
               OriginalFileName = reader.GetString(reader.GetOrdinal("OriginalFileName")),
               Status = reader.GetString(reader.GetOrdinal("Status")),
               NewFileName = reader.IsDBNull(reader.GetOrdinal("NewFileName"))
                             ? null
                             : reader.GetString(reader.GetOrdinal("NewFileName")),
               DateProcessed = reader.GetDateTime(reader.GetOrdinal("DateProcessed"))
            });
         }

         return Ok(logProcesses);
      }

      /// <summary>
      /// Obtiene el registro de la tabla LogProcess identificado por el parámetro id.
      /// </summary>
      /// <param name="id"></param>
      /// <returns>El objeto LogProcess en caso de ser encontrado.</returns>
      [HttpGet("{id}")]
      public async Task<ActionResult<IEnumerable<LogProcess>>> GetById(int id)
      {
         var logProcess = new LogProcess();

         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("GetLogProcessById", connection);
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.AddWithValue("@Id", id);
         var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
         {
            Direction = ParameterDirection.Output
         };
         command.Parameters.Add(resultCodeParam);

         using var reader = await command.ExecuteReaderAsync();

         if (reader.HasRows)
         {
            while (await reader.ReadAsync())
            {
               logProcess = new LogProcess
               {
                  Id = reader.GetInt32(reader.GetOrdinal("Id")),
                  OriginalFileName = reader.GetString(reader.GetOrdinal("OriginalFileName")),
                  Status = reader.GetString(reader.GetOrdinal("Status")),
                  NewFileName = reader.IsDBNull(reader.GetOrdinal("NewFileName"))
                             ? null
                             : reader.GetString(reader.GetOrdinal("NewFileName")),
                  DateProcessed = reader.GetDateTime(reader.GetOrdinal("DateProcessed"))
               };
            }
         }

         reader.Close();

         var returnCode = (int)resultCodeParam.Value;

         return returnCode switch
         {
            -1 => NotFound(new { message = $"LogProcess with ID {id} not found." }),
            0 => Ok(logProcess),
            _ => StatusCode(500, "Unknown error occurred.")
         };
      }

      /// <summary>
      /// Crea un LogProcess con los valores obtenidos desde el body de la petición.
      /// </summary>
      /// <param name="logProcessDto"></param>
      [HttpPost]
      public async Task<ActionResult> Post(LogProcessDto logProcessDto)
      {
         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("CreateLogProcess", connection);
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.AddWithValue("@OriginalFileName", logProcessDto.OriginalFileName);
         command.Parameters.AddWithValue("@Status", logProcessDto.Status);
         command.Parameters.AddWithValue("@NewFileName", logProcessDto.NewFileName);
         command.Parameters.AddWithValue("@DateProcessed", DateTimeOffset.Now);
         var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
         {
            Direction = ParameterDirection.Output
         };
         command.Parameters.Add(resultCodeParam);

         await command.ExecuteNonQueryAsync();
         var resultCode = (int)resultCodeParam.Value;

         if (resultCode == -1)
         {
            return StatusCode(500, "OriginalFilename, Status and DateProcessed cannot be null.");
         }
         else if (resultCode == 0)
         {
            return Ok(new { message = "LogProcess created successfully" });
         }

         return StatusCode(500, "Unknown error occurred.");
      }

      /// <summary>
      /// Actualiza un LogProcess con su ID.
      /// </summary>
      /// <param name="id"></param>
      /// <param name="logProcessDto"></param>
      [HttpPut("{id}")]
      public async Task<ActionResult> Put(int id, LogProcessDto logProcessDto)
      {
         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("UpdateLogProcess", connection);
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.AddWithValue("@Id", id);
         command.Parameters.AddWithValue("@OriginalFileName", logProcessDto.OriginalFileName);
         command.Parameters.AddWithValue("@Status", logProcessDto.Status);
         command.Parameters.AddWithValue("@NewFileName", logProcessDto.NewFileName);
         var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
         {
            Direction = ParameterDirection.Output
         };
         command.Parameters.Add(resultCodeParam);

         await command.ExecuteNonQueryAsync();
         var resultCode = (int)resultCodeParam.Value;
         return resultCode switch
         {
            -1 => NotFound(new { message = $"LogProcess with ID {id} not found." }),
            0 => Ok(new { message = "LogProcess updated successfully." }),
            _ => StatusCode(500, "Unknown error occurred.")
         };
      }

      /// <summary>
      /// Elimina un LogProcess con su ID.
      /// </summary>
      /// <param name="id"></param>
      [HttpDelete("{id}")]
      public async Task<ActionResult> Delete(int id)
      {
         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("DeleteLogProcess", connection);
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.AddWithValue("@Id", id);
         var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
         {
            Direction = ParameterDirection.Output
         };
         command.Parameters.Add(resultCodeParam);

         var rowsAffected = await command.ExecuteNonQueryAsync();
         var resultCode = (int)resultCodeParam.Value;
         return resultCode switch
         {
            -1 => NotFound(new { message = $"LogProcess with ID {id} not found." }),
            0 => Ok(new { message = "LogProcess deleted successfully." }),
            _ => StatusCode(500, "Unknown error occurred.")
         };
      }
   }
}