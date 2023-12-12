using MossWPF.Core;
using MossWPF.Core.Events;
using MossWPF.Core.Mvvm;
using MossWPF.Domain;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;

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

        private MossSubmission _mossSubmission;
        public MossSubmission MossSubmission
        {
            get { return _mossSubmission; }
            set { SetProperty(ref _mossSubmission, value); }
        }

        public ResultsBrowserViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(regionManager)
        {
            _regionManager = regionManager;
           eventAggregator.GetEvent<BackNavigationEvent>().Subscribe(BackNavigate);
            _eventAggregator = eventAggregator;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if(navigationContext.Parameters.ContainsKey(NavigationParameterKeys.ResultsLink))
            {
                ResultsSource = navigationContext.Parameters.GetValue<string>(NavigationParameterKeys.ResultsLink);
            }
            if (navigationContext.Parameters.ContainsKey(NavigationParameterKeys.MossSubmission))
            {
                MossSubmission = navigationContext.Parameters.GetValue<MossSubmission>(NavigationParameterKeys.MossSubmission);
            }

            _eventAggregator.GetEvent<CanNavigateBackEvent>().Publish(true);
        }

        
        void BackNavigate(string parameter)
        {
            var p = new NavigationParameters()
            {
                {NavigationParameterKeys.MossSubmission, MossSubmission }
            };
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView", p);
        }

    }
}
