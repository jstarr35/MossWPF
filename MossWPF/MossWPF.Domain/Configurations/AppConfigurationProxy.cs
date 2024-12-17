using MossWPF.Domain.Models;

namespace MossWPF.Domain.Configurations
{
    public class AppConfigurationProxy
    {
        //public UserOptions UserOptions { get; set; }
        public MossDefaultOptions? MossDefaultOptions { get; set; }
        public ServerSettings? ServerSettings { get; set; }
        public ScriptSettings? ScriptSettings { get; set; }
        public List<ProgrammingLanguage>? ProgrammingLanguages { get; set; }
    }

}
