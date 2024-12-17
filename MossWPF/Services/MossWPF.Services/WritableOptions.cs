using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using MossWPF.Domain.Services;


namespace MossWPF.Services
{

    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _section;
        private readonly string _file;
        private readonly object _lock = new();
        private readonly ILogger<WritableOptions<T>> _logger;

        public WritableOptions(
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string section,
            string file,
            ILogger<WritableOptions<T>> logger)
        {
            _options = options;
            _configuration = configuration;
            _section = section;
            _file = file;
            _logger = logger;
        }

        public T Value => _options.CurrentValue;
        public T Get(string name) => _options.Get(name);

        public void Update(Action<T> applyChanges)
        {
            lock (_lock)
            {
                try
                {
                    // Read the JSON file into a JsonDocument
                    using var stream = File.OpenRead(_file);
                    using var reader = new StreamReader(stream);
                    using var jsonDocument = JsonDocument.Parse(reader.ReadToEnd());
                    var section = jsonDocument.RootElement.GetProperty(_section);
                    var options = JsonSerializer.Deserialize<T>(section.GetRawText());

                    if (options != null)
                    {
                        applyChanges(options);
                    }
                    else
                    {
                        throw new InvalidOperationException("Deserialized options are null.");
                    }

                    var updatedSection = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
                    var json = jsonDocument.RootElement.GetRawText().Replace(section.GetRawText(), updatedSection);

                    File.WriteAllText(_file, json);
                    _configuration.Reload();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the configuration.");
                    throw;
                }
            }
        }
    }
}
