using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MossWPF.Domain
{
    

    public class MossDefaultOptions
    {
        public int MaxAppearances { get; set; }
        public int ResultsToDisplay { get; set; }
        public bool UseExperimental { get; set; }
        public bool UseDirectoryMode { get; set; }
    }

    public class ServerSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public int ReceiveTimeout { get; set; }
        public int SendTimeout { get; set; }
        public int ReplySize { get; set; }
    }

    public class ScriptSettings
    {
        public string FileUploadFormat { get; set; }
        public string Opt_moss { get; set; }
        public string Opt_m { get; set; }
        public string Opt_c { get; set; }
        public string Opt_n { get; set; }
        public string Opt_x { get; set; }
        public string Opt_end { get; set; }
        public string Opt_d { get; set; }
        public string Opt_l { get; set; }

    }

    public interface IAppConfiguration
    {
        //UserOptions UserOptions { get; set; }
        MossDefaultOptions MossDefaultOptions { get; set; }
        ServerSettings ServerSettings { get; set; }
        ScriptSettings ScriptSettings { get; set; }
        List<ProgrammingLanguage> ProgrammingLanguages { get; set; }
    }

    public class AppConfigurationProxy : IAppConfiguration
    {
        //public UserOptions UserOptions { get; set; }
        public MossDefaultOptions MossDefaultOptions { get; set; }
        public ServerSettings ServerSettings { get; set; }
        public ScriptSettings ScriptSettings { get; set; }
        public List<ProgrammingLanguage> ProgrammingLanguages { get; set; }
    }

    public class AppConfiguration : IAppConfiguration
    {
        //public UserOptions UserOptions { get; set; }
        public MossDefaultOptions MossDefaultOptions { get; set; }
        public ServerSettings ServerSettings { get; set; }
        public ScriptSettings ScriptSettings { get; set; }
        public List<ProgrammingLanguage> ProgrammingLanguages { get; set; } = new List<ProgrammingLanguage>();
        public AppConfiguration()
        {
            var settings = File.ReadAllText("appsettings.json");
            var config = JsonSerializer.Deserialize<AppConfigurationProxy>(settings);
           // UserOptions = config.UserOptions;
            MossDefaultOptions = config.MossDefaultOptions;
            ServerSettings = config.ServerSettings;
            ScriptSettings = config.ScriptSettings;
            foreach (var language in config.ProgrammingLanguages)
            {
                ProgrammingLanguages.Add(language);
            }
        }

        private string _filePath;
        public AppConfiguration(string filePath)
        {
            _filePath = filePath;
            var settings = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<AppConfigurationProxy>(settings);
           // UserOptions = config.UserOptions;
            MossDefaultOptions = config.MossDefaultOptions;
            ServerSettings = config.ServerSettings;
            ScriptSettings = config.ScriptSettings;
            foreach (var language in config.ProgrammingLanguages)
            {
                ProgrammingLanguages.Add(language);
            }
        }

        public async Task SaveConfiguration(CancellationToken token=default)
        {
            using var writer = File.OpenWrite(_filePath);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            await JsonSerializer.SerializeAsync(writer, this, options, token);
            await writer.DisposeAsync();
        }
    }

}
