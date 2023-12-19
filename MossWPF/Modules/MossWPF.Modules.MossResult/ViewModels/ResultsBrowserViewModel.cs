using MossWPF.Core;
using MossWPF.Core.Mvvm;
using MossWPF.Domain;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Windows;

namespace MossWPF.Modules.MossResult.ViewModels
{
    public class ResultsBrowserViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
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

        public ResultsBrowserViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            _journal = navigationContext.NavigationService.Journal;
            GoBackCommand.RaiseCanExecuteChanged();
            if(navigationContext.Parameters.ContainsKey(NavigationParameterKeys.ResultsLink))
            {
                ResultsSource = navigationContext.Parameters.GetValue<string>(NavigationParameterKeys.ResultsLink);
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

    }
}
