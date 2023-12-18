using MossWPF.Core;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using Microsoft.Extensions.Configuration;
using MossWPF.Domain;
using Prism.Events;
using MossWPF.Core.Events;
using MossWPF.Core.Dialogs;
using MaterialDesignThemes.Wpf;
using Prism.Services.Dialogs;
using System.Windows.Navigation;

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

        private bool _isHelpOpen;
        public bool IsHelpOpen
        {
            get => _isHelpOpen;
            set => SetProperty(ref _isHelpOpen, value);
        }

        public ISnackbarMessageQueue NotificationMessageQueue { get; set; }

        private DelegateCommand<string> _navigateCommand;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ISnackbarMessageQueue snackbarMessageQueue, IApplicationCommands applicationCommands, IDialogService dialogService)
        {
            NotificationMessageQueue = snackbarMessageQueue;
            _regionManager = regionManager;
            //applicationCommands.NavigateCommand.RegisterCommand(NavigateCommand);
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<SnackbarMessageEvent>().Subscribe(QueueMessage);
            //eventAggregator.GetEvent<CanNavigateBackEvent>().Subscribe((x) => { CanNavigateBack = x; });
            //eventAggregator.GetEvent<CanNavigateForwardEvent>().Subscribe((x) => { CanNavigateForward = x; });
            //_dialogService = dialogService;
            //ShowSettingsDialogIfNoUserId();

            //eventAggregator.GetEvent<RequestInitializationEvent>().Publish(new System.Collections.Generic.Dictionary<string, string>()
            //{
            //    { "UserId",Properties.Settings.Default.UserId }
            //});
        }

        private void QueueMessage(string obj)
        {
            NotificationMessageQueue.Enqueue(obj);
        }

        //void ShowSettingsDialogIfNoUserId()
        //{
        //    if(Properties.Settings.Default.UserId == "" || Properties.Settings.Default.UserId == null)
        //    {
        //        _dialogService.ShowDialog("UserSetupDialog", result =>
        //        {
        //            if (result.Result == ButtonResult.OK)
        //            {
        //                var id = result.Parameters.GetValue<string>("UserId");
        //                Properties.Settings.Default.UserId = id;
        //                Properties.Settings.Default.Save();
        //                _eventAggregator.GetEvent<RequestInitializationEvent>().Publish(new System.Collections.Generic.Dictionary<string, string>()
        //                {
        //                    { "UserId",id }
        //                });
        //            }
        //        });
        //    }
        //}



        #region Navigation
        //private bool _canNavigateBack;
        //public bool CanNavigateBack
        //{
        //    get { return _canNavigateBack; }
        //    set { SetProperty(ref _canNavigateBack, value); }
        //}

        //private bool _canNavigateForward;
        //public bool CanNavigateForward
        //{
        //    get { return _canNavigateForward; }
        //    set { SetProperty(ref _canNavigateForward, value); }
        //}

        //public DelegateCommand<string> NavigateCommand =>
        //    _navigateCommand ??= new DelegateCommand<string>(ExecuteNavigateCommand);

        //private DelegateCommand _navigateBackCommand;
        //public DelegateCommand NavigateBackCommand =>
        //    _navigateBackCommand ??= new DelegateCommand(ExecuteNavigateBackCommand);

        //private DelegateCommand _navigateForwardCommand;
        //public DelegateCommand NavigateForwardCommand =>
        //    _navigateForwardCommand ??= new DelegateCommand(ExecuteNavigateForwardCommand);

        //private void ExecuteNavigateForwardCommand()
        //{
        //    _eventAggregator.GetEvent<ForwardNavigationEvent>().Publish("ResultsBrowser");
        //}

        //void ExecuteNavigateBackCommand()
        //{
        //    _eventAggregator.GetEvent<BackNavigationEvent>().Publish("RequestBuilderView");
        //}

        //void ExecuteNavigateCommand(string navigationPath)
        //{
        //    if (string.IsNullOrEmpty(navigationPath))
        //        _regionManager.RequestNavigate(RegionNames.ContentRegion, "RequestBuilderView");
        //    else
        //        _regionManager.RequestNavigate(RegionNames.ContentRegion, navigationPath);
        //}



        //public void GoBack(object commandArg)
        //{
        //    if (NavigationService.Journal.CanGoBack)
        //    {
        //        NavigationService.Journal.GoBack();
        //    }
        //}

        //public virtual bool CanGoBack(object commandArg)
        //{
        //    return NavigationService.Journal.CanGoBack;
        //}
        #endregion

        #region UserSetupDialog
        //private DelegateCommand _openUserSetupDialog;
        //public DelegateCommand OpenUserSetupDialog =>
        //    _openUserSetupDialog ??= new DelegateCommand(ExecuteOpenUserSetupDialog);

        //void ExecuteOpenUserSetupDialog()
        //{
        //    IsUserSettingsDialogOpen = true;
        //}

        //private DelegateCommand _acceptUserSettingsDialog;
        //public DelegateCommand AcceptUserSettingsDialog =>
        //    _acceptUserSettingsDialog ??= new DelegateCommand(ExecuteAcceptUserSettingsDialog);

        //void ExecuteAcceptUserSettingsDialog()
        //{
        //    IsUserSettingsDialogOpen= false;
        //    _eventAggregator.GetEvent<RequestInitializationEvent>().Publish(new System.Collections.Generic.Dictionary<string, string>()
        //    {
        //        { "UserId",Properties.Settings.Default.UserId }
        //    });
        //}

        //private bool _isUserSettingsDialogOpen;
        //public bool IsUserSettingsDialogOpen
        //{
        //    get { return _isUserSettingsDialogOpen; }
        //    set { SetProperty(ref _isUserSettingsDialogOpen, value); }
        //}
        #endregion
    }
}
