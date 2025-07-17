using DocProcessing.API.Dtos;
using DocProcessing.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocProcessing.API.Controllers
{
   [ApiController]
   [Route("form")]
   public class FormController(DocKeywordService docKeyService) : ControllerBase
   {
      private const string FolderPath = @"C:\PruebaEQ";
      private readonly DocKeywordService _docKeyService = docKeyService;

      /// <summary>
      /// Endpoint utilizado por el frontend para la creación de un archivo PDF en la ruta C:\PruebaEQ y para crear el DocKeyword en la base de datos.
      /// </summary>
      /// <param name="formDto"></param>
      /// <returns>Código HTTP del resultado</returns>
      [HttpPost]
      public async Task<IActionResult> UploadAndRegisterDocKeyword([FromForm] FormDto formDto)
      {
         if (formDto.File is not null && formDto.File.Length > 0)
         {
            if (!Path.GetExtension(formDto.File.FileName).Equals(".pdf", StringComparison.CurrentCultureIgnoreCase))
               return BadRequest(new { message = "Solo se permiten archivos PDF." });

            if (!Directory.Exists(FolderPath))
               Directory.CreateDirectory(FolderPath);

            var filePath = Path.Combine(FolderPath, Path.GetFileName(formDto.File.FileName));

            using var stream = new FileStream(filePath, FileMode.Create);
            await formDto.File.CopyToAsync(stream);

         }

         var resultCode = await _docKeyService.CrearDocKeyAsync(formDto.DocName, formDto.Keyword);

         return resultCode switch
         {
            -1 => StatusCode(500, "Nombre o palabra clave nula."),
            0 => Ok(new { mensaje = "Cambios hechos exitosamente." }),
            _ => StatusCode(500, "Error desconocido.")
         };
      }

   }
}
