namespace MossWPF.Domain.Entities
{
    public class User : EntityBase
    {
        public string Username { get; set; } = null!;

        // Navigation Property
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
