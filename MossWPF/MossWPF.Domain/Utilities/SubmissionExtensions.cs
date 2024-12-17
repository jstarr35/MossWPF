using MossWPF.Domain.Models;

namespace MossWPF.Domain.Utilities
{
    public static class SubmissionExtensions
    {
        public static bool FilterFileItems(this MossSubmission submission)
        {
            if (submission.SelectedLanguage == null)
                return false;
            var tempSourceList = new List<FileListItem>();
            var tempBaseList = new List<FileListItem>();
            foreach (var file in submission.SourceFiles)
            {
                if (submission.SelectedLanguage.Extensions.Select(e => e.Replace(".", "")).Contains(file.Extension))
                {
                    tempSourceList.Add(file);
                }
            }
            submission.SourceFiles.Clear();
            tempSourceList.ForEach(i => submission.SourceFiles.Add(i));
            foreach (var file in submission.BaseFiles)
            {
                if (submission.SelectedLanguage.Extensions.Select(e => e.Replace(".", "")).Contains(file.Extension))
                {
                    tempBaseList.Add(file);
                }
            }
            submission.BaseFiles.Clear();
            tempBaseList.ForEach(i => submission.BaseFiles.Add(i));
            return true;
        }
    }
}
