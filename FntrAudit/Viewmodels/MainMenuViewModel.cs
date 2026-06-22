using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FntrAudit.Helpers;
using FntrAudit.Services.Activites;
using FntrAudit.Services.Auth;
using FntrAudit.Services.Clients;
using FntrAudit.Viewmodels.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace FntrAudit.Viewmodels
{
    public class MainMenuViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView == value) return;
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private bool _isMenuCollapsed;
        public bool IsMenuCollapsed
        {
            get => _isMenuCollapsed;
            set
            {
                if (_isMenuCollapsed == value) return;
                _isMenuCollapsed = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleMenuCommand { get; }

        public MainMenuViewModel()
        {
            var adminAudit = new MenuItemViewModel(
                "Audit",
                () => App.AppHost.Services.GetRequiredService<Views.AdminAuditView>(),
                OnMenuSelected);

            var adminPersonnel = new MenuItemViewModel(
                "Personnel",
                () => App.AppHost.Services.GetRequiredService<Views.AdminPersonnelView>(),
                OnMenuSelected);

            var administration = new MenuItemViewModel("Administration")
            {
                IsExpandable = true,
                IsExpanded = true
            };
            administration.Children.Add(adminAudit);
            administration.Children.Add(adminPersonnel);

            var nouvelAudit = new MenuItemViewModel(
                "Nouvel Audit",
                () => CreateClientSelectionView(ClientSelectionMode.NewAudit),
                OnMenuSelected);

            var repriseAudit = new MenuItemViewModel(
                "Reprise d'audit",
                () => CreateClientSelectionView(ClientSelectionMode.ResumeAudit),
                OnMenuSelected);

            MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                administration,
                nouvelAudit,
                repriseAudit
            };

            ToggleMenuCommand = new RelayCommand(() => IsMenuCollapsed = !IsMenuCollapsed);

            CurrentView = adminAudit.Content;
        }

        private object CreateClientSelectionView(ClientSelectionMode mode)
        {
            var clientService = App.AppHost.Services.GetRequiredService<IClientService>();
            var userSessionService = App.AppHost.Services.GetRequiredService<IUserSessionService>();
            var activityService = App.AppHost.Services.GetRequiredService<IActivityService>();

            var vm = new ClientSelectionViewModel(clientService, userSessionService, mode);

            vm.AddRequested += async () =>
            {
                var societeId = userSessionService.CurrentUser?.societeUser_Id;
                if (societeId == null)
                {
                    vm.ErrorMessage = "Aucune société utilisateur n'est disponible.";
                    return;
                }

                var dialogVm = new CreateClientViewModel(activityService, clientService, societeId.Value);
                var dialog = new Views.ClientEditDialog(dialogVm)
                {
                    Owner = Application.Current.MainWindow
                };

                if (dialog.ShowDialog() == true)
                    await vm.LoadAsync();
            };

            vm.ClientSelected += async client =>
            {
                var societeId = userSessionService.CurrentUser?.societeUser_Id;
                if (societeId == null)
                {
                    vm.ErrorMessage = "Aucune société utilisateur n'est disponible.";
                    return;
                }

                var dialogVm = new CreateClientViewModel(client, activityService, clientService, societeId.Value);
                var dialog = new Views.ClientEditDialog(dialogVm)
                {
                    Owner = Application.Current.MainWindow
                };

                if (dialog.ShowDialog() == true)
                    await vm.LoadAsync();
            };

            return new Views.ClientSelectionView(vm);
        }

        private void OnMenuSelected(MenuItemViewModel item)
        {
            CurrentView = item.Content;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}