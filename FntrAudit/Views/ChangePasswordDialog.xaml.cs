using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FntrAudit.Views
{
    /// <summary>
    /// Logique d'interaction pour ChangePasswordDialog.xaml
    /// </summary>
    public partial class ChangePasswordDialog : Window
    {
        public string? Email { get; private set; }

        public ChangePasswordDialog(string? defaultEmail = null)
        {
            InitializeComponent();
            EmailTextBox.Text = defaultEmail ?? string.Empty;
            EmailTextBox.Focus();
            EmailTextBox.SelectAll();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // ferme
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Email = EmailTextBox.Text?.Trim();
            DialogResult = true; // ferme
        }
    }
}
