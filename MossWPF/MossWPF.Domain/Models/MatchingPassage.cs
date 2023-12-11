namespace MossWPF.Domain.Models
{
    public class MatchingPassage : MossObject
    {
        public int FilePairId { get; set; }
        public string? MatchedLines { get; set; }
        public string? FirstFileLines { get; set; }
        public string? SecondFileLines { get; set; }

        public virtual FilePair FilePair { get; set; }
    }
}
