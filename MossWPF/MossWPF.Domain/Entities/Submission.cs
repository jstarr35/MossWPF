namespace MossWPF.Domain.Entities
{
    public class Submission : EntityBase
    {
        public string SubmissionName { get; set; } = null!;
        public string SubmissionUrl { get; set; } = null!;

        // Navigation Property
        public ICollection<File> Files { get; set; } = new List<File>();
        public int? UserId { get; set; }
        public User? User { get; set; }
    }

}
