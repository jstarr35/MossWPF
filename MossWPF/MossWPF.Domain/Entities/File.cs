namespace MossWPF.Domain.Entities
{
    public class File : EntityBase
    {
        public int SubmissionId { get; set; }  // Foreign Key
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;

        // Navigation Properties
        public Submission Submission { get; set; } = null!;
        public ICollection<FileComparison> FileComparisonsFile1 { get; set; } = new List<FileComparison>();
        public ICollection<FileComparison> FileComparisonsFile2 { get; set; } = new List<FileComparison>();
    }

}
