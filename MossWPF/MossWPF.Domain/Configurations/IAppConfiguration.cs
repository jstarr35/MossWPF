using MossWPF.Domain.Models;

namespace MossWPF.Domain.Configurations
{
    public interface IAppConfiguration
    {
        //UserOptions UserOptions { get; set; }
        MossDefaultOptions MossDefaultOptions { get; set; }
        ServerSettings ServerSettings { get; set; }
        ScriptSettings ScriptSettings { get; set; }
        List<ProgrammingLanguage> ProgrammingLanguages { get; set; }
    }

}
