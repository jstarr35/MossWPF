using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using DryIoc.Microsoft.DependencyInjection.Extension;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MossWPF.Core;
using MossWPF.Core.Dialogs;
using MossWPF.Data;
using MossWPF.Data.Services;
using MossWPF.Domain.Configurations;
using MossWPF.Domain.DTOs;
using MossWPF.Domain.Services;
using MossWPF.Modules.MossRequest;
using MossWPF.Modules.MossResult;
using MossWPF.Services;
using MossWPF.ViewModels;
using MossWPF.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Serilog;
using System;
using System.IO;
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
       

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File("logs\\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Set up configuration
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = configurationBuilder.Build();

            // Configure Services
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddHttpClient();
            services.AddDbContext<MossDbContext>(options =>
                options.UseSqlite(MossWPF.Properties.Settings.Default.ConnectionString));
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });
            services.AddSingleton<MossDbContextFactory>(new MossDbContextFactory(options =>
                options.UseSqlite(MossWPF.Properties.Settings.Default.ConnectionString)));
            services.AddTransient<IResultParser, ResultParser>();
            services.AddSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();
            services.AddSingleton<IAppConfiguration, AppConfiguration>();
            services.AddSingleton<IApplicationCommands, ApplicationCommands>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IFilePairService, FilePairDataService>();
            services.AddSingleton<ISubmissionFileService, SubmissionFileDataService>();

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Register services with Prism
            containerRegistry.RegisterInstance(serviceProvider.GetRequiredService<IAppConfiguration>());
            containerRegistry.RegisterInstance(serviceProvider.GetRequiredService<MossDbContextFactory>());
            containerRegistry.RegisterInstance(serviceProvider.GetRequiredService<IResultParser>());
            containerRegistry.RegisterInstance(serviceProvider.GetRequiredService<ISnackbarMessageQueue>());
            containerRegistry.RegisterInstance(serviceProvider.GetRequiredService<IApplicationCommands>());
            containerRegistry.RegisterInstance(serviceProvider.GetRequiredService<IMessageService>());
            containerRegistry.RegisterInstance(serviceProvider.GetRequiredService<IFilePairService>());
            containerRegistry.RegisterInstance(serviceProvider.GetRequiredService<ISubmissionFileService>());

            containerRegistry.RegisterForNavigation<UserSetup, UserSetupViewModel>();
        }

        protected override Window CreateShell()
        {
            using (var context = Container.Resolve<MossDbContextFactory>().CreateDbContext())
            {
                context.Database.Migrate();
            }

            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MossRequestModule>();
            moduleCatalog.AddModule<MossResultModule>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (MossWPF.Properties.Settings.Default.ShowSetup)
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
                        { NavigationParameterKeys.UserSettings, userSettings }
                    };
                    regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView", p);
                }
                else
                {
                    regionManager.RequestNavigate(RegionNames.ContentRegion, "UserSetup");
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Application shutting down.");
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.Information("Application starting up.");
        }
    }
}
