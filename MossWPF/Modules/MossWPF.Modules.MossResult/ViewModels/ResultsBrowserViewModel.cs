using MossWPF.Core;
using MossWPF.Core.Mvvm;
using MossWPF.Domain.Entities;
using MossWPF.Domain.Models;
using MossWPF.Domain.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace MossWPF.Modules.MossResult.ViewModels
{
    public class ResultsBrowserViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IFilePairService _filePairService;
        private readonly ISubmissionService _submissionService;
        private readonly IDataService<File> _fileService;
        private readonly IDataService<FileComparison> _fileComparisonService;
        private readonly IResultParser _resultParser;
        IRegionNavigationJournal _journal;

        private string _resultsSource;
        public string ResultsSource
        {
            get { return _resultsSource; }
            set { SetProperty(ref _resultsSource, value); }
        }

        private Visibility _goBackButtonVisibility;
        public Visibility GoBackButtonVisibility
        {
            get => _goBackButtonVisibility;
            set => SetProperty(ref _goBackButtonVisibility, value);
        }

        private MossSubmission _mossSubmission;
        public MossSubmission MossSubmission
        {
            get { return _mossSubmission; }
            set { SetProperty(ref _mossSubmission, value); }
        }

        private DelegateCommand _saveResultsCommand;
        public DelegateCommand SaveResultsCommand =>
            _saveResultsCommand ??= new DelegateCommand(ExecuteSaveResultsCommand);

        async void ExecuteSaveResultsCommand()
        {
            await ProcessResults();
        }

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ??= new DelegateCommand(ExecuteGoBackCommand, CanGoBack);

        private bool CanGoBack()
        {
            if (_journal != null && _journal.CanGoBack)
            {
                GoBackButtonVisibility = Visibility.Visible;
                return true;
            }
            else
            {
                GoBackButtonVisibility = Visibility.Collapsed;
                return false;
            }
        }

        void ExecuteGoBackCommand()
        {
            _journal.GoBack();
        }

        private DelegateCommand<string> _navigateCommand;

        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ??= new DelegateCommand<string>(Navigate);

        public ResultsBrowserViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IFilePairService filePairService,
            ISubmissionService submissionFileService,
            IResultParser resultParser,
            IDataService<File> fileService,
            IDataService<FileComparison> fileComparisonService)
            : base(regionManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _filePairService = filePairService;
            _submissionService = submissionFileService;
            _resultParser = resultParser;
            _fileService = fileService;
            _fileComparisonService = fileComparisonService;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            _journal = navigationContext.NavigationService.Journal;
            GoBackCommand.RaiseCanExecuteChanged();
            if(navigationContext.Parameters.ContainsKey(NavigationParameterKeys.ResultsLink))
            {
                ResultsSource = navigationContext.Parameters.GetValue<string>(NavigationParameterKeys.ResultsLink);
                Debug.WriteLine(ResultsSource);
            }
            if (navigationContext.Parameters.ContainsKey(NavigationParameterKeys.MossSubmission))
            {
                MossSubmission = navigationContext.Parameters.GetValue<MossSubmission>(NavigationParameterKeys.MossSubmission);
            }
        }

        
        void Navigate(string parameter)
        {
            var p = new NavigationParameters()
            {
                {NavigationParameterKeys.MossSubmission, MossSubmission }
            };
            _journal.GoBack();
        }

        async Task ProcessResults()
        {
            if(MossSubmission?.ResultsLink != null) 
            {
                var html = await _resultParser.DownloadHtmlAsync(MossSubmission.ResultsLink.ToString().Trim('\0').Trim());
                var results = await _resultParser.ExtractItemsAndHrefs(html);
                foreach (var result in results)
                {
                    var first = new File() { FilePath = result.FirstFilePath, SubmissionId = MossSubmission.SubmissionId };
                    var firstEntity = await _fileService.Create(first);
                    var second = new File () { FilePath = result.SecondFilePath, SubmissionId = MossSubmission.SubmissionId };
                    var secondEntity = await _fileService.Create(second);
                    var filePair = new FileComparison()
                    {
                        //FirstFile = firstEntity,
                        File1Id = firstEntity.Id,
                        //SecondFile = secondEntity,
                        File2Id = secondEntity.Id,
                        File1MatchPct = result.FirstFileScore,
                        File2MatchPct = result.SecondFileScore,
                        //LinesMatched = result.LinesMatched,
                        ComparisonUrl = result.Link,
                        SubmissionId = MossSubmission.SubmissionId
                    };
                    await _fileComparisonService.Create(filePair);
                } 
            }
        }
    }
}
