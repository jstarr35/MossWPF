using MossWPF.Core;
using MossWPF.Modules.MossResult.ViewModels;
using MossWPF.Modules.MossResult.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MossWPF.Modules.MossResult
{
    public class MossResultModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public MossResultModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(ResultsBrowser));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ResultsBrowser, ResultsBrowserViewModel>();
        }
    }
}