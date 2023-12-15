using MaterialDesignExtensions.Controls;
using MossWPF.Core;
using MossWPF.Core.Events;
using MossWPF.Core.Mvvm;
using MossWPF.Domain;
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
using System.Text;
using System.Threading.Tasks;

namespace MossWPF.Modules.MossRequest.ViewModels
{
    public class RequestBuilderViewModel : RegionViewModelBase
    {
        private readonly IAppConfiguration _config;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        #region Properties
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
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


        public DelegateCommand ClearFilesCommand =>
            _clearFilesCommand ??= new DelegateCommand(ExecuteClearFilesCommand, () => MossSubmission.SourceFiles.Any() || MossSubmission.BaseFiles.Any());

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
        
        async void ExecuteOpenBaseFileCommand()
        {
            OpenMultipleFilesDialogArguments arguments = new()
            {
                Width = 600,
                Height = 800,
                ShowHiddenFilesAndDirectories = false,
                ShowSystemFilesAndDirectories = false,
                SwitchPathPartsAsButtonsEnabled = true,
                CurrentDirectory = _config.UserOptions.DefaultFilesLocation,
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
                CurrentDirectory = _config.UserOptions.DefaultFilesLocation,
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
                    fileListItems = filtered.Select(y => y.FullName).Except(MossSubmission.SourceFiles.Select(x => x.Name)).Select(f => new FileListItem(f, 'S'));
                }
                else
                {
                    fileListItems = files.Select(y => y.FullName).Except(MossSubmission.SourceFiles.Select(x => x.Name)).Select(f => new FileListItem(f, 'S'));
                }

                MossSubmission.SourceFiles.AddRange(fileListItems);
                Files = MossSubmission?.SourceFiles;
                if (MossSubmission.SourceFiles.Any()) SendRequestCommand.RaiseCanExecuteChanged();
            }

        }
        
        void ExecuteClearFilesCommand()
        {
            MossSubmission.BaseFiles.Clear();
            MossSubmission.SourceFiles.Clear();
        }

        async void ExecuteSendRequest()
                {
                    IsBusy = true;
                    await Task.Run(() =>
                    {
                        using var mossComm = new MossCommunication(MossSubmission, _config);
                        mossComm.Connect();
                        mossComm.SendOptions();
                        string response = mossComm.ReceiveResponse(512);
                        MossSubmission.SetResultLink(response);
                        Response = response;
                        Debug.WriteLine(response);
                        mossComm.Disconnect();
                    });
                    IsBusy = false;
                    Navigate("ResultsBrowser");
                }
        
        #endregion

        

        public RequestBuilderViewModel(IRegionManager regionManager, IAppConfiguration config, IEventAggregator eventAggregator) :
            base(regionManager)
        {
            _config = config;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            MossSubmission = new MossSubmission()
            {
                Sensitivity = config.MossDefaultOptions.MaxAppearances,
                ResultsToShow = config.MossDefaultOptions.ResultsToDisplay,
                SourceFiles = new ObservableCollection<FileListItem>(),
                BaseFiles = new ObservableCollection<FileListItem>()

            };
            Languages = new List<ProgrammingLanguage>(config.ProgrammingLanguages);
            Files = MossSubmission.SourceFiles;
            
            eventAggregator.GetEvent<ForwardNavigationEvent>().Subscribe(Navigate);
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
            var p = new NavigationParameters
            {
                { NavigationParameterKeys.MossSubmission, MossSubmission },
                { NavigationParameterKeys.ResultsLink, Response.Trim('\0').Trim() }
            };
            _regionManager.RequestNavigate(RegionNames.ContentRegion, uri, p);
        }


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey(NavigationParameterKeys.MossSubmission))
            {
                MossSubmission = navigationContext.Parameters.GetValue<MossSubmission>(NavigationParameterKeys.MossSubmission);
                _eventAggregator.GetEvent<CanNavigateForwardEvent>().Publish(true);
            }
            else if (navigationContext.Parameters.ContainsKey(NavigationParameterKeys.UserId))
            {
                MossSubmission.UserId = navigationContext.Parameters.GetValue<string>(NavigationParameterKeys.UserId);
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
