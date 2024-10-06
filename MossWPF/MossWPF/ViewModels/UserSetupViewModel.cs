using MaterialDesignExtensions.Controls;
using MossWPF.Core;
using MossWPF.Core.Mvvm;
using MossWPF.Domain;
using MossWPF.Domain.DTOs;
using Prism.Commands;
using Prism.Regions;

namespace MossWPF.ViewModels
{
    public class UserSetupViewModel : RegionViewModelBase, INavigationAware
    {
        private readonly IAppConfiguration _config;
        private readonly IRegionManager _regionManager;



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
            var userSettings = new UserSettings(UserId, SubmissionsDirectory, DefaultFilesLocation);
            var p = new NavigationParameters()
            {
                {NavigationParameterKeys.UserSettings, userSettings}
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

           
        }

        async void ExecuteSelectDirectory(string type)
        {

            OpenDirectoryDialogArguments arguments = new()
            {
                Width = 600,
                Height = 800,
                CreateNewDirectoryEnabled = false,
                ShowSystemFilesAndDirectories = false,
                SwitchPathPartsAsButtonsEnabled = true,

            };
            var info = await OpenDirectoryDialog.ShowDialogAsync("FileExplorerDialogHost", arguments);
            if (!info.Canceled)
            {
                switch (type)
                {
                    case "SubmissionsDirectory":
                        Properties.Settings.Default.SubmissionsDirectory = info.Directory;
                        SubmissionsDirectory = info.Directory;
                        Properties.Settings.Default.Save();
                        break;
                    case "DefaultFilesLocation":
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
            

        }
    }
}
