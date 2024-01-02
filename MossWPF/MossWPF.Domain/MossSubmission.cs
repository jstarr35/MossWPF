using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MossWPF.Domain
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
    public class MossSubmission : BindableBase
    {
        public Guid Id { get; set; }

        private bool _isLanguageSelected;
        public bool IsLanguageSelected
        {
            get { return _isLanguageSelected; }
            set { SetProperty(ref _isLanguageSelected, value); }
        }


        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        private ProgrammingLanguage _selectedLanguage;
        public ProgrammingLanguage SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                SetProperty(ref _selectedLanguage, value);
                IsLanguageSelected = true;
            }
        }

        private int _sensitivity;
        public int Sensitivity
        {
            get => _sensitivity;
            set => SetProperty(ref _sensitivity, value);
        }

        private int _resultsToShow;
        public int ResultsToShow
        {
            get => _resultsToShow;
            set => SetProperty(ref _resultsToShow, value);
        }

        private bool _useDirectoryMode;
        public bool UseDirectoryMode
        {
            get => _useDirectoryMode;
            set => SetProperty(ref _useDirectoryMode, value);
        }

        private bool _useExperimental;
        public bool UseExperimental
        {
            get => _useExperimental;
            set => SetProperty(ref _useExperimental, value);
        }

        private string? _comments;
        public string Comments
        {
            get => _comments ?? string.Empty;
            set => SetProperty(ref _comments, value);
        }

        private string? _title;
        public string Title
        {
            get => _title ?? string.Empty;
            set => SetProperty(ref _title, value);
        }

        private DateTime _dateSubmitted;
        public DateTime DateSubmitted
        {
            get => _dateSubmitted;
            set => SetProperty(ref _dateSubmitted, value);
        }

        private DateTime _dateCreated;
        public DateTime DateCreated
        {
            get => _dateCreated;
            set => SetProperty(ref _dateCreated, value);
        }

        private ObservableCollection<FileListItem> _sourceFiles;
        public ObservableCollection<FileListItem> SourceFiles
        {
            get => _sourceFiles;
            set => SetProperty(ref _sourceFiles, value);
        }

        private ObservableCollection<FileListItem> _baseFiles;
        public ObservableCollection<FileListItem> BaseFiles
        {
            get => _baseFiles;
            set => SetProperty(ref _baseFiles, value);
        }

        private Uri? _resultsLink;
        public Uri ResultsLink
        {
            get { return _resultsLink!; }
            set { SetProperty(ref _resultsLink, value); }
        }

        public void SetResultLink(string link)
        {
            Uri.TryCreate(link, UriKind.Absolute, out _resultsLink);
        }


        public MossSubmission()
        {

        }
    }
}
