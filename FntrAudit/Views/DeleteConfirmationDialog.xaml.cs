using System.Windows;

namespace FntrAudit.Views
{
    public partial class DeleteConfirmationDialog : Window
    {
        public DeleteConfirmationDialog(string message, string titleText = "Confirmer la suppression")
        {
            InitializeComponent();
            DataContext = new DeleteConfirmationDialogData(titleText, message);
        }

        public static bool Confirm(Window? owner, string message, string titleText = "Confirmer la suppression")
        {
            var dialog = new DeleteConfirmationDialog(message, titleText);

            if (owner != null)
                dialog.Owner = owner;

            return dialog.ShowDialog() == true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private sealed record DeleteConfirmationDialogData(string TitleText, string Message);
    }
}