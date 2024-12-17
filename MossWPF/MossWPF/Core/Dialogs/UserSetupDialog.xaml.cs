using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MossWPF.Core.Dialogs
{
    /// <summary>
    /// Interaction logic for UserSetupDialog.xaml
    /// </summary>
    public partial class UserSetupDialog : UserControl
    {
        public UserSetupDialog()
        {
            InitializeComponent();
        }

        private void DoNotShowCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowSetup = false;
        }

        private void UserSetupDialog_OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(UserIdTextBox.Text) && int.TryParse(UserIdTextBox.Text, out int id))
            {
                Properties.Settings.Default.UserId = id.ToString();
                Properties.Settings.Default.Save();
            }

        }
    }
}
