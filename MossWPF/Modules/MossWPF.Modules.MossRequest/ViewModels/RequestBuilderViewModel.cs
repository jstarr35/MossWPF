using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
using MossWPF.Core;
using MossWPF.Core.Events;
using MossWPF.Core.Mvvm;
using MossWPF.Domain.Configurations;
using MossWPF.Domain.DTOs;
using MossWPF.Domain.Models;
using MossWPF.Domain.Utilities;
using MossWPF.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MossWPF.Modules.MossRequest.ViewModels
{
    public class RequestBuilderViewModel : RegionViewModelBase
    {
        private readonly IAppConfiguration _config;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        IRegionNavigationJournal _journal;
        private string _submissionsDirectory;
        private string _defaultFilesLocation;

        public ISnackbarMessageQueue SnackbarMessageQueue { get; set; }

        #region Properties
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private Visibility _goForwardButtonVisibility;
        public Visibility GoForwardButtonVisibility
        {
            get => _goForwardButtonVisibility;
            set => SetProperty(ref _goForwardButtonVisibility, value);
        }

        private bool _filterFileTypes;
        public bool FilterFileTypes
        {
            get { return _filterFileTypes; }
            set
            {
                MossSubmission.FilterFileItems();
                SetProperty(ref _filterFileTypes, value);
            }
        }

        private string _response;
        public string Response
        {
            get { return _response; }
            set { SetProperty(ref _response, value); }
        }

        private bool _languageSelected;
        public bool LanguageSelected
        {
            get { return _languageSelected; }
            set { SetProperty(ref _languageSelected, value); }
        }

        private MossSubmission _mossSubmission;
        public MossSubmission MossSubmission
        {
            get { return _mossSubmission; }
            set { SetProperty(ref _mossSubmission, value); }
        }

        private ObservableCollection<FileListItem> _files;
        public ObservableCollection<FileListItem> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }
        }

        public List<ProgrammingLanguage> Languages { get; private set; }
        #endregion

        #region Commands

        private DelegateCommand _openSourceFilesDirectoryCommand;
        private DelegateCommand _openBaseFileCommand;
        private DelegateCommand _sendRequest;
        private DelegateCommand _showSourceFilesCommand;
        private DelegateCommand _showBaseFilesCommand;
        private DelegateCommand _clearFilesCommand;
        private DelegateCommand<string> _navigateCommand;
        private DelegateCommand _goForwardCommand;

        public DelegateCommand GoForwardCommand =>
            _goForwardCommand ??= new DelegateCommand(ExecuteGoForwardCommand, CanGoForward);

        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ??= new DelegateCommand<string>(Navigate);
        
        public DelegateCommand ClearFilesCommand =>
            _clearFilesCommand ??= new DelegateCommand(ExecuteClearFilesCommand, CanClearFiles)
            .ObservesProperty(() => Files)
            .ObservesProperty(() => MossSubmission.BaseFiles);

        public DelegateCommand ShowBaseFilesCommand =>
            _showBaseFilesCommand ??= new DelegateCommand(() => { Files = MossSubmission?.BaseFiles; });

        public DelegateCommand ShowSourceFilesCommand =>
            _showSourceFilesCommand ??= new DelegateCommand(() => { Files = MossSubmission?.SourceFiles; });

        public DelegateCommand OpenBaseFileCommand =>
            _openBaseFileCommand ??= new DelegateCommand(ExecuteOpenBaseFileCommand);

        public DelegateCommand OpenSourceFilesDirectoryCommand =>
            _openSourceFilesDirectoryCommand ??= new DelegateCommand(ExecuteOpenSourceFilesDirectoryCommand);

        public DelegateCommand SendRequestCommand =>
            _sendRequest ??= new DelegateCommand(ExecuteSendRequest, IsValidForm)
            .ObservesProperty(() => MossSubmission.SelectedLanguage);

        #endregion Commands

        #region Actions

        void ExecuteGoForwardCommand()
        {
            _journal.GoForward();
        }

        async void ExecuteOpenBaseFileCommand()
        {
            OpenMultipleFilesDialogArguments arguments = new()
            {
                Width = 600,
                Height = 800,
                ShowHiddenFilesAndDirectories = false,
                ShowSystemFilesAndDirectories = false,
                SwitchPathPartsAsButtonsEnabled = true,
                CurrentDirectory = _defaultFilesLocation,
                Filters = GetFileTypeFilter()
            };
            var info = await OpenMultipleFilesDialog.ShowDialogAsync("FileExplorerDialogHost", arguments);
            if (!info.Canceled)
            {
                info.Files.ForEach(f => MossSubmission.BaseFiles.Add(new FileListItem(f, 'B')));
                Files = MossSubmission?.BaseFiles;
            }
        }
        
        async void ExecuteOpenSourceFilesDirectoryCommand()
        {
            OpenDirectoryDialogArguments arguments = new()
            {
                Width = 600,
                Height = 800,
                CreateNewDirectoryEnabled = false,
                CurrentDirectory = _defaultFilesLocation,
                ShowSystemFilesAndDirectories = false,
                SwitchPathPartsAsButtonsEnabled = true,

            };
            var info = await OpenDirectoryDialog.ShowDialogAsync("FileExplorerDialogHost", arguments);
            
            if (!info.Canceled)
            {
                IEnumerable<FileListItem> fileListItems;
                var files = info.DirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();

                if (FilterFileTypes)
                {
                    var exts = MossSubmission.SelectedLanguage.Extensions.Select(ex => ex.ToString());
                    var filtered = files.Where(f => exts.Contains(f.Extension));
                    filtered.ToList().ForEach(f => Debug.WriteLine(f.FullName));
                    MossSubmission.SourceFiles.ToList().ForEach(f => Debug.WriteLine(f.Path));
                    fileListItems = filtered.Select(y => y.FullName).Except(MossSubmission.SourceFiles.Select(x => x.Path)).Select(f => new FileListItem(f, 'S'));
                }
                else
                {
                    fileListItems = files.Select(y => y.FullName).Except(MossSubmission.SourceFiles.Select(x => x.Name)).Select(f => new FileListItem(f, 'S'));
                }

                MossSubmission.SourceFiles.AddRange(fileListItems);
                Files = MossSubmission?.SourceFiles;
                if (MossSubmission.SourceFiles.Any())
                {
                    SendRequestCommand.RaiseCanExecuteChanged();
                    ClearFilesCommand.RaiseCanExecuteChanged();
                }
            }
        }
        
        void ExecuteClearFilesCommand()
        {
            MossSubmission.BaseFiles.Clear();
            MossSubmission.SourceFiles.Clear();
            ClearFilesCommand.RaiseCanExecuteChanged();
        }

        async void ExecuteSendRequest()
        {
            IsBusy = true;
            MossSocketResult mossCommResult = null;

            using var mossComm = new MossCommunication(MossSubmission, _config);
            if ((await HandleCommError(() => mossComm.TryConnectAsync())).Success)
            {
                if ((await HandleCommError(() => mossComm.TrySendOptions())).Success)
                {
                    mossCommResult = await HandleCommError(() => mossComm.TryReceiveResponseAsync());
                    if (mossCommResult.Success)
                    {
                        MossSubmission.SetResultLink(mossCommResult.Response);
                        Response = mossCommResult.Response;
                    }
                }
            }

            Debug.WriteLine((await mossComm.DisconnectAsync()).ErrorMessage ?? "Socket disconnected successfully");

            IsBusy = false;

            MossSubmission.DateSubmitted = DateTime.Now;
            if (mossCommResult is not null && mossCommResult.Success)
                Navigate("ResultsBrowser");
            
            
        }

        private DelegateCommand _snackBarTestCommand;
        public DelegateCommand SnackBarTestCommand =>
            _snackBarTestCommand ??= new DelegateCommand(ExecuteSnackBarTestCommand);

        void ExecuteSnackBarTestCommand()
        {
           // SnackbarMessageQueue.Enqueue("Test button clicked.");
            _eventAggregator.GetEvent<SnackbarMessageEvent>().Publish("Test button clicked.");
        }
        #endregion


        public RequestBuilderViewModel(
            IRegionManager regionManager,
            IAppConfiguration config,
            IEventAggregator eventAggregator,
            ISnackbarMessageQueue snackbarMessageQueue) :
            base(regionManager)
        {
           SnackbarMessageQueue = snackbarMessageQueue;
            Debug.WriteLine("Constructing RequestBuilderViewModel");
            _config = config;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            MossSubmission = new MossSubmission()
            {
                Id = Guid.NewGuid(),
                Sensitivity = config.MossDefaultOptions.MaxAppearances,
                ResultsToShow = config.MossDefaultOptions.ResultsToDisplay,
                SourceFiles = new ObservableCollection<FileListItem>(),
                BaseFiles = new ObservableCollection<FileListItem>(),
                DateCreated = DateTime.Now

            };
            Languages = new List<ProgrammingLanguage>(config.ProgrammingLanguages);
            Files = MossSubmission.SourceFiles;
            _eventAggregator.GetEvent<SaveSubmissionEvent>().Subscribe(async () => await SaveSubmissionAsync());
        }

        private async Task SaveSubmissionAsync()
        {
            var submissionService = new SubmissionService();
            var path = !string.IsNullOrWhiteSpace(_submissionsDirectory) ? _submissionsDirectory : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            await submissionService.SaveSubmissionAsync(MossSubmission, path);
        }

        private async Task<MossSocketResult> HandleCommError(Func<Task<MossSocketResult>> communicationAction)
        {
            var result = await communicationAction();
            if (!result.Success)
            {
                _eventAggregator.GetEvent<SnackbarMessageEvent>().Publish(result.ErrorMessage);
                
            }
            return result;
                
            
        }

        private bool CanGoForward()
        {
            if (_journal != null && _journal.CanGoForward)
            {
                GoForwardButtonVisibility = Visibility.Visible;
                return true;
            }
            else
            {
                GoForwardButtonVisibility = Visibility.Collapsed;
                return false;
            }
        }

        private string GetFileTypeFilter()
        {
            var filterStringBuilder = new StringBuilder();
            if(FilterFileTypes)
            {
                if (MossSubmission.SelectedLanguage != null)
                {
                    filterStringBuilder.Append(MossSubmission.SelectedLanguage.Name).Append(" Files|");
                    for (int i = 0; i < MossSubmission.SelectedLanguage.Extensions.Count; i++)
                    {
                        filterStringBuilder.Append('*').Append(MossSubmission.SelectedLanguage.Extensions[i]);
                        if (i < MossSubmission.SelectedLanguage.Extensions.Count - 1)
                        {
                            filterStringBuilder.Append(';');
                        }
                        else
                        {
                            filterStringBuilder.Append("|All Files|*.*");
                        }
                    }
                }
            }
            return filterStringBuilder.ToString();
        }

        private bool CanClearFiles()
        {
            if (MossSubmission.SourceFiles.Any() || MossSubmission.BaseFiles.Any())
                return true;
            else
                return false;
        }
        
        private bool IsValidForm()
        {
            if(!MossSubmission.SourceFiles.Any()) 
            {
                return false;
            }
            if(MossSubmission.SelectedLanguage == null)
            { 
                return false;
            }
            return true;
        }

        private void Navigate(string uri)
        {
            var p = new NavigationParameters();
            if (Response is not null)
            {
                p.Add(NavigationParameterKeys.MossSubmission, MossSubmission);
                p.Add(NavigationParameterKeys.ResultsLink, Response.Trim('\0').Trim());
            }
            NavigationService.RequestNavigate(uri, p);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            _journal = navigationContext.NavigationService.Journal;
            GoForwardCommand.RaiseCanExecuteChanged();
            if (navigationContext.Parameters.ContainsKey(NavigationParameterKeys.MossSubmission))
            {
                MossSubmission = navigationContext.Parameters.GetValue<MossSubmission>(NavigationParameterKeys.MossSubmission);             
            }
            else if (navigationContext.Parameters.ContainsKey(NavigationParameterKeys.UserSettings))
            {
                var userSettings = navigationContext.Parameters.GetValue<UserSettings>(NavigationParameterKeys.UserSettings);
                MossSubmission.UserId = userSettings.UserId;
                if (userSettings.SubmissionsDirectory != null)
                    _submissionsDirectory = userSettings.SubmissionsDirectory;

                if (userSettings.DefaultFilesLocation != null)
                    _defaultFilesLocation = userSettings.DefaultFilesLocation;
            }
        }

        public static async Task UploadFileAsync(string file, int id, string lang, StreamWriter fileWriter)
        {
            int size;
            using (var reader = new StreamReader(file))
            {
                size = (await reader.ReadToEndAsync()).Length;
            }

            Debug.WriteLine($"Uploading {file} ...");

            await fileWriter.WriteLineAsync($"file {id} {lang} {size} {file}");

            using (StreamReader reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    await fileWriter.WriteLineAsync(await reader.ReadLineAsync());
                }
            }

            // Do not flush here
            Console.WriteLine("Done uploading file.");
        }
    }
}
