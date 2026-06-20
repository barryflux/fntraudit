using System.Windows;
using FntrAudit.Viewmodels;

namespace FntrAudit.Views
{
    public partial class ActivityEditDialog : Window
    {
        public ActivityEditDialog(ActivityEditDialogViewModel viewModel)
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