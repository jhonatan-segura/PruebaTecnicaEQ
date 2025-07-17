using DocProcessing.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocProcessing.API.Data
{
   public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
   {
      public DbSet<DocKeyword> DocKeywords { get; set; }
   }
}