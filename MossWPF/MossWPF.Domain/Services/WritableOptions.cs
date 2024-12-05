using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text.Json;

namespace MossWPF.Domain.Services
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        void Update(Action<T> applyChanges);
    }
    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _section;
        private readonly string _file;

        public WritableOptions(
            
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string section,
            string file)
        {
            
            _options = options;
            _configuration = configuration;
            _section = section;
            _file = file;
        }

        public T Value => _options.CurrentValue;
        public T Get(string name) => _options.Get(name);

        public void Update(Action<T> applyChanges)
        {

            // Read the JSON file into a JsonDocument
            using (var stream = File.OpenRead(_file))
            using (var reader = new StreamReader(stream))
            {
                using (var document = JsonDocument.Parse(reader.ReadToEnd()))
                {
                    // Get the root object
                    var rootObject = document.RootElement;

                    // Get the specified section or create a new one
                    var sectionObject = rootObject.TryGetProperty(_section, out var section)
                        ? JsonSerializer.Deserialize<T>(section.ToString())
                        : Value ?? new T();

                    // Apply changes to the section object
                    applyChanges(sectionObject);

                    // Update the root object with the modified section
                    rootObject.TryGetProperty(_section, out section);
                    section = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(sectionObject));

                    // Write the modified JsonDocument back to the file
                    using (var outputStream = new MemoryStream())
                    using (var writer = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true }))
                    {
                        rootObject.WriteTo(writer);
                        writer.Flush();
                        File.WriteAllBytes(_file, outputStream.ToArray());
                    }
                }
            }

            // Reload the configuration (if _configuration is IConfigurationRoot)
            _configuration.Reload();
        }
    }


}
