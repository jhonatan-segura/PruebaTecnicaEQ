using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DocProcessing.API.Entities;
using DocProcessing.API.Dtos;
using DocProcessing.API.Services;

namespace DocProcessing.API.Controllers
{

   [ApiController]
   [Route("DocKeyword")]
   public class DocKeywordController : ControllerBase
   {
      private readonly IConfiguration _config;
      private readonly DocKeywordService _docKeyService;

      public DocKeywordController(IConfiguration config, DocKeywordService docKeyService)
      {
         _config = config;
         _docKeyService = docKeyService;
      }

      [HttpGet]
      public async Task<ActionResult<IEnumerable<DocKeyword>>> Get()
      {
         var docKeywords = new List<DocKeyword>();

         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("GetDocKeywords", connection);
         command.CommandType = CommandType.StoredProcedure;

         using var reader = await command.ExecuteReaderAsync();
         while (await reader.ReadAsync())
         {
            docKeywords.Add(new()
            {
               Id = reader.GetInt32(reader.GetOrdinal("Id")),
               DocName = reader.GetString(reader.GetOrdinal("DocName")),
               Keyword = reader.GetString(reader.GetOrdinal("Keyword"))
            });
         }

         return Ok(docKeywords);
      }

      [HttpGet("{id}")]
      public async Task<ActionResult<IEnumerable<DocKeyword>>> GetById(int id)
      {
         var docKeyword = new DocKeyword();

         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("GetDocKeywordById", connection);
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
               docKeyword = new DocKeyword
               {
                  Id = reader.GetInt32(reader.GetOrdinal("Id")),
                  DocName = reader.GetString(reader.GetOrdinal("DocName")),
                  Keyword = reader.GetString(reader.GetOrdinal("Keyword"))
               };
            }
         }

         reader.Close();

         var returnCode = (int)resultCodeParam.Value;

         return returnCode switch
         {
            -1 => NotFound(new { message = $"DocKey with ID {id} not found." }),
            0 => Ok(docKeyword),
            _ => StatusCode(500, "Unknown error occurred.")
         };
      }

      [HttpPost]
      public async Task<ActionResult> Post(DocKeywordDto docKeywordDto)
      {
         var resultCode = await _docKeyService.CrearDocKeyAsync(docKeywordDto.DocName, docKeywordDto.Keyword);

         return resultCode switch
         {
            -1 => StatusCode(500, "Nor DocName neither Keyword can be null."),
            0 => Ok(new { message = "DocKey created successfully" }),
            _ => StatusCode(500, "Unknown error occurred.")
         };
      }

      [HttpPut("{id}")]
      public async Task<ActionResult> Put(int id, DocKeywordDto docKeywordDto)
      {
         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("UpdateDocKeyword", connection);
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.AddWithValue("@Id", id);
         command.Parameters.AddWithValue("@DocName", docKeywordDto.DocName);
         command.Parameters.AddWithValue("@Keyword", docKeywordDto.Keyword);
         var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
         {
            Direction = ParameterDirection.Output
         };
         command.Parameters.Add(resultCodeParam);

         await command.ExecuteNonQueryAsync();
         var resultCode = (int)resultCodeParam.Value;
         return resultCode switch
         {
            -1 => NotFound(new { message = $"DocKey with ID {id} not found." }),
            0 => Ok(new { message = "DocKey updated successfully." }),
            _ => StatusCode(500, "Unknown error occurred.")
         };
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      [HttpDelete("{id}")]
      public async Task<ActionResult> Delete(int id)
      {
         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("DeleteDocKeyword", connection);
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
            -1 => NotFound(new { message = $"DocKey with ID {id} not found." }),
            0 => Ok(new { message = "DocKey deleted successfully." }),
            _ => StatusCode(500, "Unknown error occurred.")
         };
      }
   }
}