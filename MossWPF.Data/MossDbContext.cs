using Microsoft.EntityFrameworkCore;
using MossWPF.Domain.Models;

namespace MossWPF.Data
{
    public class MossDbContext : DbContext
    {
        public MossDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SubmissionFile> Files { get; set; }
        public DbSet<FilePair> FilesPairs { get; set; }
        public DbSet<MatchingPassage> MatchingPassages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=moss.db");
        }
    }
}
