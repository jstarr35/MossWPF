using MossWPF.Domain.Models;

namespace MossWPF.Domain.Services
{
    public interface ISubmissionService
    {
        Task SaveSubmissionAsync(MossSubmission mossSubmission, string path);
    }
}