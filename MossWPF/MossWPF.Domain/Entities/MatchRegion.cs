namespace MossWPF.Domain.Entities
{
    public class MatchRegion : EntityBase
    {
        public int ComparisonId { get; set; }  // Foreign Key
        public int File1Start { get; set; }
        public int File1End { get; set; }
        public int File2Start { get; set; }
        public int File2End { get; set; }

        // Navigation Property
        public FileComparison FileComparison { get; set; } = null!;
    }

}
