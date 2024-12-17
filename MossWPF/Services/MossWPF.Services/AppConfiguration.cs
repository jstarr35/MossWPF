using Microsoft.Extensions.Configuration;
using MossWPF.Domain.Configurations;
using MossWPF.Domain.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MossWPF.Services
{

    public class AppConfiguration : IAppConfiguration
    {
        //public UserOptions UserOptions { get; set; }
        public MossDefaultOptions MossDefaultOptions { get; set; } = new MossDefaultOptions();
        public ServerSettings ServerSettings { get; set; } = new ServerSettings();
        public ScriptSettings ScriptSettings { get; set; } = new ScriptSettings();
        public List<ProgrammingLanguage> ProgrammingLanguages { get; set; } = new List<ProgrammingLanguage>();

        public AppConfiguration(IConfiguration configuration)
        {
            configuration.Bind(this);
        }

       
        public async Task SaveConfiguration(CancellationToken token = default)
        {
            using var writer = File.OpenWrite("appsettings.json");
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
