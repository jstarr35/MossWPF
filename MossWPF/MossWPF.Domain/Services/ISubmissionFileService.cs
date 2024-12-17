using MossWPF.Domain.Models;

namespace MossWPF.Domain.Services
{
    public interface ISubmissionFileService : IDataService<SubmissionFile>
    {
        Task<SubmissionFile> GetSubmissionFileByPath(string filePath);
    }
}
