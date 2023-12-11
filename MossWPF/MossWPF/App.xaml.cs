using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MossWPF.Core;
using MossWPF.Domain;
using MossWPF.Modules.MossRequest;
using MossWPF.Modules.MossResult;
using MossWPF.Services;
using MossWPF.Services.Interfaces;
using MossWPF.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

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
            containerRegistry.RegisterInstance(serverSettings);
            containerRegistry.RegisterSingleton<IAppConfiguration, AppConfiguration>();
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MossRequestModule>();
            moduleCatalog.AddModule<MossResultModule>();
        }
    }
}
