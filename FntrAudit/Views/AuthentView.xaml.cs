using System.Windows;
using FntrAudit.Viewmodels;
using Microsoft.Extensions.DependencyInjection;

namespace FntrAudit.Views
{
    public partial class AuthentView : Window
    {
        private readonly AuthentificationViewModel _vm;

        public AuthentView(AuthentificationViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = _vm;

            _vm.LoginSucceeded += OnLoginSucceeded;
            _vm.ForgotPasswordRequested += OnForgotPasswordRequested;
        }

        private void OnLoginSucceeded()
        {
            var mainMenu = App.AppHost.Services.GetRequiredService<MainMenu>();

            Application.Current.MainWindow = mainMenu;
            mainMenu.Show();
            Close();
        }

        private void OnForgotPasswordRequested()
        {
            var dialog = new ChangePasswordDialog(_vm.Email)
            {
                Owner = this
            };

            dialog.ShowDialog();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _vm.LoginSucceeded -= OnLoginSucceeded;
            _vm.ForgotPasswordRequested -= OnForgotPasswordRequested;
            base.OnClosed(e);
        }
    }
}