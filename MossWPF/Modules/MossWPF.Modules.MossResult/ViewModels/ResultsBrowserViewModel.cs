using MossWPF.Core.Mvvm;
using MossWPF.Domain;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace MossWPF.Modules.MossResult.ViewModels
{
    public class ResultsBrowserViewModel : RegionViewModelBase
    {
        
        private string _resultsSource;
        public string ResultsSource
        {
            get { return _resultsSource; }
            set { SetProperty(ref _resultsSource, value); }
        }

        public ResultsBrowserViewModel(IRegionManager regionManager) : base(regionManager)
        {
           
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if(navigationContext.Parameters.ContainsKey("resultsLink"))
            {
                ResultsSource = navigationContext.Parameters.GetValue<string>("resultsLink");
            }
        }

    }
}
