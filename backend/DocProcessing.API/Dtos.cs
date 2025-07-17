namespace DocProcessing.API.Dtos
{
   public record DocKeywordDto(int Id, string DocName, string Keyword);
   public record LogProcessDto(int Id, string OriginalFileName, string Status, string NewFileName, DateTime DateProcessed);
   public record FormDto(string DocName, string Keyword, IFormFile? File);
}