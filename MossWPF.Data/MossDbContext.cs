using Microsoft.EntityFrameworkCore;
using MossWPF.Data.Utils;
using MossWPF.Domain.Entities;
using MossWPF.Domain.Models;
using File = MossWPF.Domain.Entities.File;

namespace MossWPF.Data
{
    public class MossDbContext : DbContext
    {
        public MossDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SubmissionSettings> SubmissionSettings { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FileComparison> FileComparisons { get; set; }
        public DbSet<MatchRegion> MatchRegions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQLite Database Connection
            optionsBuilder.UseSqlite("Data Source=moss.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply value converter for SourceFiles
            modelBuilder.Entity<SubmissionSettings>()
                .Property(s => s.SourceFiles)
                .HasConversion(new JsonValueConverter<FileListItem>());

            // Apply value converter for BaseFiles
            modelBuilder.Entity<SubmissionSettings>()
                .Property(s => s.BaseFiles)
                .HasConversion(new JsonValueConverter<FileListItem>());

            // FileComparisons relationships
            modelBuilder.Entity<FileComparison>()
                .HasOne(fc => fc.File1)
                .WithMany(f => f.FileComparisonsFile1)
                .HasForeignKey(fc => fc.File1Id)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascading delete on File1

            modelBuilder.Entity<FileComparison>()
                .HasOne(fc => fc.File2)
                .WithMany(f => f.FileComparisonsFile2)
                .HasForeignKey(fc => fc.File2Id)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascading delete on File2

            modelBuilder.Entity<File>()
                .HasOne(f => f.Submission)
                .WithMany(s => s.Files)
                .HasForeignKey(f => f.SubmissionId);

            // MatchPassages relationship
            modelBuilder.Entity<MatchRegion>()
                .HasOne(mp => mp.FileComparison)
                .WithMany(fc => fc.MatchRegions)
                .HasForeignKey(mp => mp.ComparisonId);
        }
    }
}
