using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FntrAudit.Helpers;
using FntrAudit.Models;
using FntrAudit.Services.Auth;
using FntrAudit.Services.Clients;
using FntrAudit.Viewmodels.Common;

namespace FntrAudit.Viewmodels
{
    public class ClientSelectionViewModel : INotifyPropertyChanged
    {
        private readonly IClientService _clientService;
        private readonly IUserSessionService _userSessionService;
        private readonly ClientSelectionMode _mode;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<Client>? ClientSelected;
        public event Action? AddRequested;

        public ObservableCollection<EntityRowViewModel> Rows { get; } = new();

        public string Title =>
            _mode == ClientSelectionMode.NewAudit
                ? "Sélection d'un client pour un nouvel audit"
                : "Sélection d'un client pour une reprise d'audit";

        private EntityRowViewModel? _selectedRow;
        public EntityRowViewModel? SelectedRow
        {
            get => _selectedRow;
            set
            {
                if (_selectedRow == value) return;
                _selectedRow = value;
                OnPropertyChanged();
                _openCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                _addCommand.RaiseCanExecuteChanged();
                _openCommand.RaiseCanExecuteChanged();
                _reloadCommand.RaiseCanExecuteChanged();
            }
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage == value) return;
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private readonly RelayCommand _addCommand;
        public ICommand AddCommand => _addCommand;

        private readonly RelayCommand _openCommand;
        public ICommand OpenCommand => _openCommand;

        private readonly RelayCommand _reloadCommand;
        public ICommand ReloadCommand => _reloadCommand;

        public ClientSelectionViewModel(
            IClientService clientService,
            IUserSessionService userSessionService,
            ClientSelectionMode mode)
        {
            _clientService = clientService;
            _userSessionService = userSessionService;
            _mode = mode;

            _addCommand = new RelayCommand(Add, () => !IsBusy);
            _openCommand = new RelayCommand(Open, () => !IsBusy && SelectedRow?.Item is Client);
            _reloadCommand = new RelayCommand(() => _ = LoadAsync(), () => !IsBusy);

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;
                Rows.Clear();

                var currentUser = _userSessionService.CurrentUser;
                if (currentUser?.societeUser_Id == null)
                {
                    ErrorMessage = "Aucune société utilisateur n'est disponible.";
                    return;
                }

                List<Client> clients;

                if (_mode == ClientSelectionMode.NewAudit)
                {
                    clients = await _clientService.GetClientsBySocieteAsync(currentUser.societeUser_Id.Value);
                }
                else
                {
                    clients = await _clientService.GetClientsWithAuditToResumeBySocieteAsync(currentUser.societeUser_Id.Value);
                }

                foreach (var client in clients)
                {
                    var title = BuildClientTitle(client);
                    var subtitle = BuildClientSubtitle(client);

                    Rows.Add(new EntityRowViewModel(title, client, subtitle));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des clients : {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Add()
        {
            AddRequested?.Invoke();
        }

        private void Open()
        {
            if (SelectedRow?.Item is not Client client)
                return;

            ClientSelected?.Invoke(client);
        }

        private static string BuildClientTitle(Client client)
        {
            if (!string.IsNullOrWhiteSpace(client.raisonSociale))
                return client.raisonSociale;

            var fullName = $"{client.intitule}".Trim();
            return !string.IsNullOrWhiteSpace(fullName) ? fullName : "Client";
        }

        private static string BuildClientSubtitle(Client client)
        {
            return client.adresse
                   ?? client.siret
                   ?? client.email
                   ?? string.Empty;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}