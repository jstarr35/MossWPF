using MossWPF.Domain;

namespace MossWPF.Services.Interaces
{
    public interface ISubmissionService
    {
        Task SaveSubmissionAsync(MossSubmission mossSubmission, string path);
    }
}