using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using MossWPF.Domain.DTOs;
using MossWPF.Domain.Services;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MossWPF.Services
{
    
    public class ResultParser : IResultParser
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResultParser(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> DownloadHtmlAsync(string url)
        {
            using var client = _httpClientFactory.CreateClient();
            try
            {
                // Download HTML content
                string htmlContent = await client.GetStringAsync(url);
                return htmlContent;
            }
            catch (HttpRequestException ex)
            {
                return $"Error downloading HTML: {ex.Message}";
            }
        }

        public async Task<List<ResultTableItem>> ExtractItemsAndHrefs(string html)
        {
            var filePairs = new List<ResultTableItem>();

            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(html);


            // Assuming the table is the first table in the document
            var table = document.QuerySelector("table");

            if (table != null)
            {
                var rows = table.QuerySelectorAll("tr").Skip(1); // Skip header row
                foreach (var row in rows)
                {
                    var columns = row.QuerySelectorAll("td");

                    if (columns.Length >= 2)
                    {
                        ExtractFilePathAndPercentage(columns[0].TextContent.Trim(), out string firstFilePath, out int firstFileScore);
                        ExtractFilePathAndPercentage(columns[0].TextContent.Trim(), out string secondFilePath, out int secondFileScore);
                        var linesMatched = columns[2].TextContent.Trim();
                        var href = columns[0].QuerySelector("a")?.GetAttribute("href");

                        if (!string.IsNullOrWhiteSpace(firstFilePath) && !string.IsNullOrWhiteSpace(secondFilePath) && !string.IsNullOrWhiteSpace(href) && !string.IsNullOrWhiteSpace(linesMatched) && int.TryParse(linesMatched, out int linesMatchedValue))
                        {
                            filePairs.Add(new ResultTableItem
                            (
                                firstFilePath,
                                secondFilePath,
                                firstFileScore,
                                secondFileScore,
                                linesMatchedValue,
                                href
                            ));
                        }
                    }
                }
            }

            return filePairs;
        }

        public static void ExtractFilePathAndPercentage(string input, out string filePath, out int percentage)
        {
            // Define a regular expression pattern to match the file path and percentage
            string pattern = @"^(.*?)(\s*\((\d+)%\))";

            // Use Regex.Match to find the first match in the input string
            Match match = Regex.Match(input, pattern);

            // Initialize output variables
            filePath = string.Empty;
            percentage = 0;

            // Check if a match is found
            if (match.Success)
            {
                // Extract the file path from the first capturing group
                filePath = match.Groups[1].Value.Trim();

                // Extract the percentage from the third capturing group
                percentage = int.Parse(match.Groups[3].Value);
            }
        }

         static void RemoveFontTags(IElement element)
        {
            var fontElements = element.QuerySelectorAll("font");
            foreach (var fontElement in fontElements)
            {
                fontElement.Remove();
            }
        }

        static string ExtractFontContent(IElement element)
        {
            var fontElements = element.QuerySelectorAll("font");
            var fontContent = string.Join(Environment.NewLine, fontElements.Select(font => font.TextContent));
            return fontContent;
        }
    }
}
