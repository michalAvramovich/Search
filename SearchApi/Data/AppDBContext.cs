using Microsoft.EntityFrameworkCore;
using SearchApi.Models.Entities;

namespace SearchApi.Data
{
    public class AppDBContext : DbContext
    {
        public DbSet<SearchResult> SearchResults { get; set; }
        public AppDBContext(DbContextOptions options) : base(options)
        {
                
        }
    }
}
