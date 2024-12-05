using MossWPF.Core;
using MossWPF.Core.Mvvm;
using MossWPF.Domain;
using MossWPF.Domain.Models;
using MossWPF.Domain.Services;
using MossWPF.Services;
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
        private readonly ISubmissionFileService _submissionFileService;
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
            ISubmissionFileService submissionFileService,
            IResultParser resultParser)
            : base(regionManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _filePairService = filePairService;
            _submissionFileService = submissionFileService;
            _resultParser = resultParser;
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
                    var first = new SubmissionFile() { FilePath = result.FirstFilePath, SubmissionId = MossSubmission.Id };
                    var firstEntity = await _submissionFileService.Create(first);
                    var second = new SubmissionFile() { FilePath = result.SecondFilePath, SubmissionId = MossSubmission.Id };
                    var secondEntity = await _submissionFileService.Create(second);
                    var filePair = new FilePair()
                    {
                        //FirstFile = firstEntity,
                        FirstFileId = firstEntity.Id,
                        //SecondFile = secondEntity,
                        SecondFileId = secondEntity.Id,
                        FirstFilePercentageScore = result.FirstFileScore,
                        SecondFilePercentageScore = result.SecondFileScore,
                        LinesMatched = result.LinesMatched,
                        Link = result.Link,
                        SubmissionId = MossSubmission.Id
                    };
                    await _filePairService.Create(filePair);
                } 
            }
        }
    }
}
