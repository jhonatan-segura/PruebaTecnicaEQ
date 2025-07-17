using System.Data;
using Microsoft.Data.SqlClient;

namespace DocProcessing.API.Services
{

   public class DocKeywordService(IConfiguration config)
   {
      private readonly IConfiguration _config = config;

      public async Task<int> CrearDocKeyAsync(string docName, string keyword)
      {
         var connectionString = _config.GetConnectionString("DefaultConnection");

         using var connection = new SqlConnection(connectionString);
         await connection.OpenAsync();

         using var command = new SqlCommand("CreateDocKeyword", connection);
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.AddWithValue("@DocName", docName);
         command.Parameters.AddWithValue("@Keyword", keyword);

         var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
         {
            Direction = ParameterDirection.Output
         };
         command.Parameters.Add(resultCodeParam);

         await command.ExecuteNonQueryAsync();
         return (int)resultCodeParam.Value;
      }
   }

}