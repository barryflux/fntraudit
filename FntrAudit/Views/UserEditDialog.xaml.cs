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
using FntrAudit.Viewmodels;

namespace FntrAudit.Views
{
    /// <summary>
    /// Logique d'interaction pour UserEditDialog.xaml
    /// </summary>
    public partial class UserEditDialog : Window
    {
        public UserEditDialogViewModel ViewModel { get; }

        public UserEditDialog(UserEditDialogViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;
            DataContext = ViewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not UserEditDialogViewModel vm)
                return;

            if (!vm.Validate())
                return;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
