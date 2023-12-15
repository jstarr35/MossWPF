using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MossWPF.Core;
using MossWPF.Core.Dialogs;
using MossWPF.Domain;
using MossWPF.Modules.MossRequest;
using MossWPF.Modules.MossResult;
using MossWPF.Services;
using MossWPF.Services.Interfaces;
using MossWPF.ViewModels;
using MossWPF.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Windows;
using System.Windows.Controls;

namespace MossWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json");

            //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            ServerSettings serverSettings = new ServerSettings();
            builder.Configuration.GetSection(nameof(ServerSettings)).Bind(serverSettings);
            builder.Build();
            //containerRegistry.RegisterDialog<UserSetupDialog, UserSetupDialogViewModel>();
            containerRegistry.RegisterForNavigation<UserSetup, UserSetupViewModel>();
            containerRegistry.Register<IAppConfiguration, AppConfiguration>();
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MossRequestModule>();
            moduleCatalog.AddModule<MossResultModule>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if(MossWPF.Properties.Settings.Default.ShowSetup)
            {
                var regionManager = Container.Resolve<IRegionManager>();
                var contentRegion = regionManager.Regions["ContentRegion"];

                if (MossWPF.Properties.Settings.Default.UserId != null)
                {
                    var p = new NavigationParameters()
                    {
                        {NavigationParameterKeys.UserId, MossWPF.Properties.Settings.Default.UserId }
                    };
                    regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView");
                }
                regionManager.RequestNavigate(RegionNames.ContentRegion, "UserSetup");
            }
            
            
        }
    }
}
