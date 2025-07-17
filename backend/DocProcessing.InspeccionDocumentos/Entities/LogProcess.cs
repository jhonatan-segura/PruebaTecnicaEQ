namespace DocProcessing.API.Entities
{
   public class LogProcess
   {
      public int Id { get; set; }
      public string OriginalFileName { get; set; } = "";
      public string Status { get; set; } = "";
      public string? NewFileName { get; set; } = "";
      public DateTime DateProcessed { get; set; }
   }
}