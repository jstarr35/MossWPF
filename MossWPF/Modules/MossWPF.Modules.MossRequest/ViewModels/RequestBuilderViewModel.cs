using MaterialDesignExtensions.Controls;
using MossWPF.Core;
using MossWPF.Core.Mvvm;
using MossWPF.Domain;
using MossWPF.Services;
using MossWPF.Services.Interfaces;
using Prism.Commands;
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

        #region Commands

        public DelegateCommand LanguageSelectedCommand;

        private DelegateCommand _openBaseFileCommand;
        public DelegateCommand OpenBaseFileCommand =>
            _openBaseFileCommand ??= new DelegateCommand(ExecuteOpenBaseFileCommand);

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
            if(!info.Canceled)
            {
                info.Files.ForEach(f => MossSubmission.BaseFiles.Add(new FileListItem(f, 'B')));
            }
        }

        private DelegateCommand _openSourceFilesDirectoryCommand;
        public DelegateCommand OpenSourceFilesDirectoryCommand =>
            _openSourceFilesDirectoryCommand ??= new DelegateCommand(ExecuteOpenSourceFilesDirectoryCommand);

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
                
                if (MossSubmission.SourceFiles.Any()) SendRequestCommand.RaiseCanExecuteChanged();
            }
           
        }

        private DelegateCommand _showSourceFiles;
        public DelegateCommand ShowSourceFiles =>
            _showSourceFiles ??= new DelegateCommand(ExecuteShowSourceFiles);

        void ExecuteShowSourceFiles()
        {
            Files = MossSubmission.SourceFiles;
        }

        private DelegateCommand _showBaseFiles;
        public DelegateCommand ShowBaseFiles =>
            _showBaseFiles ??= new DelegateCommand(ExecuteShowBaseFiles);

        void ExecuteShowBaseFiles()
        {
            Files = MossSubmission.BaseFiles;
        }

        private DelegateCommand _clearFilesCommand;
        public DelegateCommand ClearFilesCommand =>
            _clearFilesCommand ??= new DelegateCommand(ExecuteClearFilesCommand);

        void ExecuteClearFilesCommand()
        {
            MossSubmission.BaseFiles.Clear();
            MossSubmission.SourceFiles.Clear();
        }

        private DelegateCommand _sendRequest;
        public DelegateCommand SendRequestCommand =>
            _sendRequest ??= new DelegateCommand(ExecuteSendRequest, IsValidForm)
            .ObservesProperty(() => MossSubmission.SelectedLanguage);

        public async Task UploadFileAsync(string file, int id, string lang, StreamWriter fileWriter)
        {
            int size;
            using (StreamReader reader = new StreamReader(file))
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
        
        #endregion Commands


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

        public RequestBuilderViewModel(IRegionManager regionManager, IAppConfiguration config) :
            base(regionManager)
        {
            _config = config;
            MossSubmission = new MossSubmission()
            {
                Sensitivity = config.MossDefaultOptions.MaxAppearances,
                ResultsToShow = config.MossDefaultOptions.ResultsToDisplay,
                SourceFiles = new ObservableCollection<FileListItem>(),
                BaseFiles = new ObservableCollection<FileListItem>()

            };
            Languages = new List<ProgrammingLanguage>(config.ProgrammingLanguages);
            Files = MossSubmission.SourceFiles;
            LanguageSelectedCommand = new DelegateCommand(() => { LanguageSelected = true; });
            _regionManager = regionManager;
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
            }
        }
    }
}
