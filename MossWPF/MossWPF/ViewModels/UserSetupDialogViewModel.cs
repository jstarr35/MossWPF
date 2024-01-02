using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace MossWPF.ViewModels
{
    public class UserSetupDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "Registration";

        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        public event Action<IDialogResult> RequestClose;

        private DelegateCommand _closeDialogCommand;
        public DelegateCommand CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand(CloseDialog);

        private void CloseDialog()
        {
            var result = ButtonResult.OK;

            var p = new DialogParameters()
            {
                {"UserId", UserId }
            };

            RequestClose?.Invoke(new DialogResult(result, p));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
