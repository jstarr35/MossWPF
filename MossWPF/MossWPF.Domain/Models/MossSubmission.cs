using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MossWPF.Domain.Models
{
    public class MossSubmission : BindableBase
    {
        public Guid Id { get; set; }
        public int SubmissionId { get; set; }
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

        private List<FileListItem> _sourceFiles = new();
        public List<FileListItem> SourceFiles
        {
            get => _sourceFiles;
            set => SetProperty(ref _sourceFiles, value);
        }

        private List<FileListItem> _baseFiles = new();
        public List<FileListItem> BaseFiles
        {
            get => _baseFiles;
            set => SetProperty(ref _baseFiles, value);
        }

        public void AddToSourceFiles(IEnumerable<FileListItem> items)
        {
            _sourceFiles.AddRange(items);
            RaisePropertyChanged(nameof(SourceFiles)); // Notify UI of the change
        }

        public void AddToBaseFiles(IEnumerable<FileListItem> items)
        {
            _baseFiles.AddRange(items);
            RaisePropertyChanged(nameof(BaseFiles)); // Notify UI of the change
        }

        public void ClearSourceFiles()
        {
            _sourceFiles.Clear();
            RaisePropertyChanged(nameof(SourceFiles)); // Notify UI of the change
        }

        public void ClearBaseFiles()
        {
            _baseFiles.Clear();
            RaisePropertyChanged(nameof(BaseFiles)); // Notify UI of the change
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
            Id = Guid.NewGuid();
            DateCreated = DateTime.Now;
        }

    }
}
