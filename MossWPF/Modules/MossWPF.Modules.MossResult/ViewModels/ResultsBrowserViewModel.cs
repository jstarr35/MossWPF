using MossWPF.Core;
using MossWPF.Core.Events;
using MossWPF.Core.Mvvm;
using MossWPF.Domain;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;

namespace MossWPF.Modules.MossResult.ViewModels
{
    public class ResultsBrowserViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

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

        private DelegateCommand<string> _navigateCommand;

        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ??= new DelegateCommand<string>(Navigate);

        public ResultsBrowserViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager)
        {
            _regionManager = regionManager;
           //eventAggregator.GetEvent<BackNavigationEvent>().Subscribe(BackNavigate);
            _eventAggregator = eventAggregator;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if(navigationContext.Parameters.ContainsKey(NavigationParameterKeys.ResultsLink))
            {
                ResultsSource = navigationContext.Parameters.GetValue<string>(NavigationParameterKeys.ResultsLink);
            }
            if (navigationContext.Parameters.ContainsKey(NavigationParameterKeys.MossSubmission))
            {
                MossSubmission = navigationContext.Parameters.GetValue<MossSubmission>(NavigationParameterKeys.MossSubmission);
            }

            GoBackButtonVisibility = NavigationService.Journal.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            //_eventAggregator.GetEvent<CanNavigateBackEvent>().Publish(true);
        }

        
        void Navigate(string parameter)
        {
            var p = new NavigationParameters()
            {
                {NavigationParameterKeys.MossSubmission, MossSubmission }
            };
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView", p);
        }

    }
}
