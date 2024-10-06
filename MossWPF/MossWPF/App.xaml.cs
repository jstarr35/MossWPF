using DryIoc.Microsoft.DependencyInjection.Extension;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MossWPF.Core;
using MossWPF.Core.Dialogs;
using MossWPF.Data;
using MossWPF.Data.Services;
using MossWPF.Domain;
using MossWPF.Domain.DTOs;
using MossWPF.Domain.Services;
using MossWPF.Modules.MossRequest;
using MossWPF.Modules.MossResult;
using MossWPF.Services;
using MossWPF.Services.Interfaces;
using MossWPF.ViewModels;
using MossWPF.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Net.Http;
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
            using (var context = Container.Resolve<MossDbContextFactory>().CreateDbContext())
            {
                context.Database.Migrate();
            }

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

            //var dbContextBuilder = new DbContextOptionsBuilder<MossDbContext>();
            //dbContextBuilder.UseSqlite(MossWPF.Properties.Settings.Default.ConnectionString);
            ////containerRegistry.RegisterInstance(dbContextBuilder.Options);
            //containerRegistry.Register<MossDbContext>();
            //containerRegistry.Register<HttpClient>();
            //containerRegistry.Register<IResultParser, ResultParser>();
            ////containerRegistry.RegisterDialog<UserSetupDialog, UserSetupDialogViewModel>();
            //containerRegistry.RegisterSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();
            //containerRegistry.RegisterForNavigation<UserSetup, UserSetupViewModel>();
            //containerRegistry.Register<IAppConfiguration, AppConfiguration>();
            //containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
            //containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            //containerRegistry.RegisterSingleton<IFilePairService, FilePairDataService>();
            //containerRegistry.RegisterSingleton<ISubmissionFileService, SubmissionFileDataService>();
            Action<DbContextOptionsBuilder> configureDbContext = o => o.UseSqlite(MossWPF.Properties.Settings.Default.ConnectionString);
            containerRegistry.GetContainer().RegisterServices(services =>
            {
                services.AddHttpClient();
                services.AddDbContext<MossDbContext>(configureDbContext);
                
                services.AddSingleton<MossDbContextFactory>(new MossDbContextFactory(configureDbContext));
                services.AddTransient<IResultParser, ResultParser>();
                services.AddSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();
                services.AddTransient<IAppConfiguration, AppConfiguration>();
                services.AddSingleton<IApplicationCommands, ApplicationCommands>();
                services.AddSingleton<IMessageService, MessageService>();
                services.AddSingleton<IFilePairService, FilePairDataService>();
                services.AddSingleton<ISubmissionFileService, SubmissionFileDataService>();
            });
            containerRegistry.RegisterForNavigation<UserSetup, UserSetupViewModel>();

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
                    var userSettings = new UserSettings(MossWPF.Properties.Settings.Default.UserId,
                        MossWPF.Properties.Settings.Default.SubmissionsDirectory,
                        MossWPF.Properties.Settings.Default.DefaultFileLocation);

                    var p = new NavigationParameters()
                    {
                        {NavigationParameterKeys.UserSettings, userSettings }
                    };
                    regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView",p);
                }
                else
                {
                    regionManager.RequestNavigate(RegionNames.ContentRegion, "UserSetup");
                }
               
            }
            
            
        }
    }
}
