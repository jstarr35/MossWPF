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

        private DelegateCommand _saveSubmissionCommand;
        public DelegateCommand SaveSubmissionCommand =>
            _saveSubmissionCommand ??= new DelegateCommand(ExecuteSaveSubmissionCommand);

        void ExecuteSaveSubmissionCommand()
        {
            _eventAggregator.GetEvent<SaveSubmissionEvent>().Publish();
        }

        private DelegateCommand<string> _navigateCommand;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ISnackbarMessageQueue snackbarMessageQueue, IApplicationCommands applicationCommands, IDialogService dialogService)
        {
            NotificationMessageQueue = snackbarMessageQueue;
            _regionManager = regionManager;
           
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<SnackbarMessageEvent>().Subscribe(QueueMessage);
            
        }

        private void QueueMessage(string obj)
        {
            NotificationMessageQueue.Enqueue(obj);
        }

    }
}
