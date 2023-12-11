namespace MossWPF.Domain.Models
{
    public class FilePair : MossObject
    {
        public int FirstFileId { get; set; }
        public virtual SubmissionFile FirstFile { get; set; }
        public int SecondFileId { get; set; }
        public virtual SubmissionFile SecondFile { get; set; }
        public int LinesMatched { get; set; }
        public int PercentageScore { get; set; }
    }
}
