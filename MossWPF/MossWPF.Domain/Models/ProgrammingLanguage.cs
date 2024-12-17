namespace MossWPF.Domain.Models
{
    public class ProgrammingLanguage
    {
        public string Name { get; set; }
        public char Icon { get; set; }
        public string Code { get; set; }
        public List<string> Extensions { get; set; }
    }
}
