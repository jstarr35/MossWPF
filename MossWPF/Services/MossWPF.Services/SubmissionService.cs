using AngleSharp;
using MossWPF.Domain;
using MossWPF.Services.Interaces;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MossWPF.Services
{
    public class SubmissionService : ISubmissionService
    {
        public async Task SaveSubmissionAsync(MossSubmission submission, string path) 
        {
            var date = (submission.DateSubmitted < submission.DateCreated) ? submission.DateCreated : submission.DateSubmitted;
            var fileName = Path.Combine(path, date.ToString("u").Replace(':', '.') + ".json");
            using var fs = new FileStream(fileName, FileMode.Create);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            await JsonSerializer.SerializeAsync<MossSubmission>(fs, submission, options);
        }
        
        public async Task ParseResultsAsync()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = "http://moss.stanford.edu/results/8/6230899184915/";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);
            var cellSelector = "body > table > tbody > tr:nth-child(2) > td:nth-child(1) > a";
            var cells = document.QuerySelectorAll(cellSelector);
            var titles = cells.Select(m => m.TextContent);
        }
        
    }
}
