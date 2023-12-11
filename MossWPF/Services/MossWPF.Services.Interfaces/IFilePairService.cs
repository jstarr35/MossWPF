using MossWPF.Domain.Models;

namespace MossWPF.Services.Interfaces
{
    public interface IFilePairService : IDataService<FilePair>
    {
        Task<FilePair> GetFilePairAsync();
    }
}
