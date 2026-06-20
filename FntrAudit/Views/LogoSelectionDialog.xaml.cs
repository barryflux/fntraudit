using System.Windows;
using FntrAudit.Viewmodels;

namespace FntrAudit.Views
{
    public partial class LogoSelectionDialog : Window
    {
        public LogoSelectionDialog(LogoSelectionDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.RequestClose += OnRequestClose;
        }

        private void OnRequestClose(bool dialogResult)
        {
            DialogResult = dialogResult;
            Close();
        }
    }
}