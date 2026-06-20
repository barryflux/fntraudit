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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FntrAudit.Models;
using FntrAudit.Services.Auth;
using FntrAudit.Services.Personnel;
using FntrAudit.Viewmodels;

namespace FntrAudit.Views
{
    /// <summary>
    /// Logique d'interaction pour AdminPersonnelView.xaml
    /// </summary>
    public partial class AdminPersonnelView : UserControl
    {
        private readonly AdminPersonnelViewModel _vm;
        private readonly IUserSessionService _userSessionService;
        private readonly IPersonnelService _personnelService;

        public AdminPersonnelView(
            AdminPersonnelViewModel vm,
            IUserSessionService userSessionService,
            IPersonnelService personnelService)
        {
            InitializeComponent();

            _vm = vm;
            _userSessionService = userSessionService;
            _personnelService = personnelService;

            DataContext = _vm;

            _vm.AddRequested += OnAddRequested;
            _vm.EditRequested += OnEditRequested;
        }

        private async void OnAddRequested()
        {
            var vm = new UserEditDialogViewModel();
            var dialog = new UserEditDialog(vm);

            if (System.Windows.Window.GetWindow(this) is System.Windows.Window owner)
                dialog.Owner = owner;

            if (dialog.ShowDialog() != true)
                return;

            var currentUser = _userSessionService.CurrentUser;
            if (currentUser?.idSociete == null)
                return;

            User user = vm.BuildUser(currentUser.idSociete.Value);
            await _personnelService.AddUserAsync(user);
            await _vm.ReloadAsync();
        }

        private async void OnEditRequested(User user)
        {
            var vm = new UserEditDialogViewModel(user);
            var dialog = new UserEditDialog(vm);

            if (System.Windows.Window.GetWindow(this) is System.Windows.Window owner)
                dialog.Owner = owner;

            if (dialog.ShowDialog() != true)
                return;

            User updatedUser = vm.BuildUser(user.idSociete ?? 0);
            await _personnelService.UpdateUserAsync(updatedUser);
            await _vm.ReloadAsync();
        }
    }
}
