using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace MossWPF.Data.Utils
{
    public class JsonValueConverter<T> : ValueConverter<List<T>, string>
    {
        public JsonValueConverter() : base(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = false }),
            v => JsonSerializer.Deserialize<List<T>>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<T>())
        {
        }
    }
}
