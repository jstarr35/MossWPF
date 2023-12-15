using MaterialDesignExtensions.Controls;
using MossWPF.Core;
using MossWPF.Core.Mvvm;
using MossWPF.Domain;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MossWPF.ViewModels
{
    public class UserSetupViewModel : RegionViewModelBase, INavigationAware
    {
        private readonly IAppConfiguration _config;
        private readonly IRegionManager _regionManager;

       // public DelegateCommand<string> ChooseDirectoryCommand;

        private DelegateCommand<string> _chooseDirectoryCommand;
        public DelegateCommand<string> ChooseDirectoryCommand =>
            _chooseDirectoryCommand ??= new DelegateCommand<string>(ExecuteSelectDirectory);
        private DelegateCommand _acceptCommand;
        public DelegateCommand AcceptCommand =>
            _acceptCommand ??= new DelegateCommand(ExecuteAcceptCommand);

        void ExecuteAcceptCommand()
        {
            Properties.Settings.Default.SubmissionsDirectory = SubmissionsDirectory;
            Properties.Settings.Default.DefaultFileLocation = DefaultFilesLocation;
            Properties.Settings.Default.UserId = UserId;
            Properties.Settings.Default.Save();
            var p = new NavigationParameters()
            {
                {NavigationParameterKeys.UserId, UserId}
            };
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView",p);
        }
        private string _userId;
        public string UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private string _defaultFilesLocation;
        public string DefaultFilesLocation
        {
            get => _defaultFilesLocation;
            set => SetProperty(ref _defaultFilesLocation, value);
        }

        private string _submissionsDirectory;
        public string SubmissionsDirectory
        {
            get => _submissionsDirectory;
            set => SetProperty(ref _submissionsDirectory, value);
        }

        public UserSetupViewModel(IAppConfiguration config, IRegionManager regionManager) :
            base(regionManager)
        {
            _config = config;
            _regionManager = regionManager;

            //ChooseDirectoryCommand = new DelegateCommand<string>(ExecuteSelectDirectory);
        }

        async void ExecuteSelectDirectory(string type)
        {

            OpenDirectoryDialogArguments arguments = new()
            {
                Width = 600,
                Height = 800,
                CreateNewDirectoryEnabled = false,
                CurrentDirectory = _config.UserOptions.DefaultFilesLocation,
                ShowSystemFilesAndDirectories = false,
                SwitchPathPartsAsButtonsEnabled = true,

            };
            var info = await OpenDirectoryDialog.ShowDialogAsync("FileExplorerDialogHost", arguments);
            if (!info.Canceled)
            {
                switch (type)
                {
                    case "SubmissionsDirectory":
                        _config.UserOptions.SubmissionsDirectory = info.Directory;
                        Properties.Settings.Default.SubmissionsDirectory = info.Directory;
                        SubmissionsDirectory = info.Directory;
                        Properties.Settings.Default.Save();
                        break;
                    case "DefaultFilesLocation":
                        _config.UserOptions.DefaultFilesLocation = info.Directory;
                        Properties.Settings.Default.DefaultFileLocation = info.Directory;
                        DefaultFilesLocation = info.Directory;
                        Properties.Settings.Default.Save();
                        break;
                    default:
                        break;
                }
            }

        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (Properties.Settings.Default.UserId != null) 
            {
                UserId = Properties.Settings.Default.UserId;
            }

        }
    }
}
