using System.Text;
using System.Text.Json;
using DocProcessing.API.Entities;
using DocProcessing.InspeccionDocumentos.Entities;

namespace DocProcessing.InspeccionDocumentos.Utils
{
   public class PdfReader
   {
      private static readonly string rootFolder = @"C:\PruebaEQ\";
      private static readonly string knownFolder = @"C:\PruebaEQ\OCR\";
      private static readonly string unknownFolder = @"C:\PruebaEQ\UNKNOWN\";

      /// <summary>
      /// Categoriza archivos .pdf presentes en la carpeta raíz dependiendo de si el texto de cada archivo contiene alguna de las palabras clave 
      /// que se encuentran en la lista de palabras clave obtenida de la base de datos.
      /// </summary>
      /// <param name="client"></param>
      /// <param name="docKeywords"></param>
      /// <returns>No se retorna un objeto a utilizar.</returns>
      public static async Task OCRAsync(HttpClient client, List<DocKeyword> docKeywords)
      {
         foreach (var file in Directory.GetFiles(rootFolder))
         {
            (bool containsKeyword, string newFileName) = ContainsKeyword(file, docKeywords);

            if (containsKeyword)
            {
               // Palabra clave encontrada, renombrar y mover el archivo
               if (!Directory.Exists(knownFolder))
               {
                  Directory.CreateDirectory(knownFolder);
               }
               string newFile = GetAvailableName(knownFolder, $"{newFileName}.pdf");
               File.Move(file, newFile);
               // Aquí se debe insertar el registro en LogProcess(estado: Processed)

               LogProcess logProcess = new()
               {
                  OriginalFileName = Path.GetFileNameWithoutExtension(file),
                  Status = "processed",
                  NewFileName = Path.GetFileNameWithoutExtension(newFile)
               };

               await RegisterLogProcess(client, logProcess);
            }
            else
            {
               if (!Directory.Exists(unknownFolder))
               {
                  Directory.CreateDirectory(unknownFolder);
               }
               // Palabra no encontrada, mover archivo a carpeta UNKNOWN
               string newFilePath = GetAvailableName(unknownFolder,Path.GetFileName(file));
               File.Move(file, newFilePath);
               // Aquí se debe insertar el registro en LogProcess(estado: unknown)
               LogProcess logProcess = new()
               {
                  OriginalFileName = Path.GetFileNameWithoutExtension(file),
                  Status = "unknown",
                  NewFileName = newFileName
               };

               await RegisterLogProcess(client, logProcess);
            }

         }
      }

      /// <summary>
      /// Evalua si las palabras clave (<paramref name="keywords"/>) están presentes en el texto del archivo ubicado en <paramref name="pdfPath"/>.
      /// En caso de haber coincidencia se concatena una cadena de texto separada por "_" que contiene el nombre final del archivo PDF procesado.
      /// </summary>
      /// <param name="pdfPath"></param>
      /// <param name="keywords"></param>
      /// <returns>Verdadero si hay al menos una coincidencia junto con el nombre final del archivo. De lo contrario se retorna falso y una cadena de texto vacía.</returns>
      public static (bool, string) ContainsKeyword(string pdfPath, List<DocKeyword> keywords)
      {
         using var pdfReader = new iText.Kernel.Pdf.PdfReader(pdfPath);
         using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader);
         string text = "";
         string newFileName = "";
         int coincidenceCount = 0;

         for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
         {
            var page = pdfDoc.GetPage(i);
            text += iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page);
         }

         bool containsAny = keywords.Any(k =>
         {
            bool contains = text.Contains(k.Keyword, StringComparison.OrdinalIgnoreCase);
            if (contains)
            {
               if (coincidenceCount++ > 1)
               {
                  newFileName += "_";
               }
               newFileName += k.DocName;
            }

            return contains;
         });
         return (containsAny, newFileName);
      }

      /// <summary>
      /// Obtiene un nuevo nombre de archivo en caso de que ya exista el obtenido por parámetro.
      /// </summary>
      /// <param name="folder"></param>
      /// <param name="baseName"></param>
      /// <returns>Un nuevo nombre de archivo con la siguiente versión expresada de forma numérica y entre paréntesis. <br/><br/>
      /// Ejemplo: <br/>
      ///   - ejemplo.pdf<br/>
      ///   - ejemplo (1).pdf<br/>
      ///   - ejemplo (2).pdf<br/>
      /// </returns>
      public static string GetAvailableName(string folder, string baseName)
      {
         string nameWithoutExtension = Path.GetFileNameWithoutExtension(baseName);
         string extension = Path.GetExtension(baseName);

         string destination = Path.Combine(folder, baseName);
         int counter = 1;

         while (File.Exists(destination))
         {
            string nuevoNombre = $"{nameWithoutExtension} ({counter}){extension}";
            destination = Path.Combine(folder, nuevoNombre);
            counter++;
         }

         return destination;
      }

      /// <summary>
      /// Registra un objeto logProcess en la base de datos.
      /// </summary>
      /// <param name="client"></param>
      /// <param name="logProcess"></param>
      /// <returns>No retorna un objeto.</returns>
      private static async Task RegisterLogProcess(HttpClient client, LogProcess logProcess)
      {
         string json = JsonSerializer.Serialize(logProcess);

         var content = new StringContent(json, Encoding.UTF8, "application/json");
         if (client != null)
         {
            var response = await client.PostAsync("LogProcess", content);
            string responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Código de estado: {response.StatusCode}");
            Console.WriteLine($"Respuesta: {responseContent}");
         }
         else
         {
            Console.WriteLine("Error: HttpClient is not initialized.");
         }
      }
   }
}