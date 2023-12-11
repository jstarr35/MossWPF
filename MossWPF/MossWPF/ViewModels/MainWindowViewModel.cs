using MossWPF.Core;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using Microsoft.Extensions.Configuration;
using MossWPF.Domain;

namespace MossWPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Windows Moss Client";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _canNavigateBack;
        public bool CanNavigateBack
        {
            get { return _canNavigateBack; }
            set { SetProperty(ref _canNavigateBack, value); }
        }

        private DelegateCommand<string> _navigateCommand;
        private readonly IRegionManager _regionManager;

        private readonly IConfigurationRoot _config;
        private readonly ServerSettings _serverSettings;
        private readonly IAppConfiguration _appConfiguration;

        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ??= new DelegateCommand<string>(ExecuteNavigateCommand);

        public MainWindowViewModel(IRegionManager regionManager, IApplicationCommands applicationCommands, IAppConfiguration config)
        {
            _regionManager = regionManager;
            applicationCommands.NavigateCommand.RegisterCommand(NavigateCommand);
            _appConfiguration = config;
        }

        void ExecuteNavigateCommand(string navigationPath)
        {
            if (string.IsNullOrEmpty(navigationPath))
                _regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView");

            _regionManager.RequestNavigate(RegionNames.ContentRegion, navigationPath);
        }
    }
}
