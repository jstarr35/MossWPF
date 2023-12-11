using MossWPF.Core;
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
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(RequestBuilderView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            //_regionManager.RegisterViewWithRegion(nameof(RequestBuilderView),typeof(RequestBuilderView));
            //IRegion region = _regionManager.Regions[RegionNames.ContentRegion];
            //var requestBuilderView = containerProvider.Resolve<RequestBuilderView>();
            //region.Add(requestBuilderView);

            //_regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<RequestBuilderView, RequestBuilderView>();
        }
    }
}