using MossWPF.Domain.DTOs;

namespace MossWPF.Domain.Services
{
    public interface IResultParser
    {
        Task<string> DownloadHtmlAsync(string url);
        Task<List<ResultTableItem>> ExtractItemsAndHrefs(string html);
    }
}
