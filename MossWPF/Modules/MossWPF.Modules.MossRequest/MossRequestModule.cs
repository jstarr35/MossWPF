using MossWPF.Core;
using MossWPF.Modules.MossRequest.ViewModels;
using MossWPF.Modules.MossRequest.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MossWPF.Modules.MossRequest
{
    public class MossRequestModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public MossRequestModule(IRegionManager regionManager)
        {
            //_regionManager = regionManager;
            //_regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(RequestBuilderView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<RequestBuilderView, RequestBuilderViewModel>();
        }
    }
}