namespace MossWPF.Domain.Entities
{
    public class FileComparison : EntityBase
    {
        public int SubmissionId { get; set; }  // Foreign Key
        public int File1Id { get; set; }  // Foreign Key
        public int File2Id { get; set; }  // Foreign Key
        public double Similarity { get; set; }
        public double File1MatchPct { get; set; } 
        public double File2MatchPct { get; set; } 
        public string ComparisonUrl { get; set; } = null!;

        // Navigation Properties
        public Submission Submission { get; set; } = null!;
        public File File1 { get; set; } = null!;
        public File File2 { get; set; } = null!;

        public ICollection<MatchRegion> MatchRegions { get; set; } = new List<MatchRegion>();
    }

}
