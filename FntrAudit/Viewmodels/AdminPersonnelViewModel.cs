using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FntrAudit.Helpers;
using FntrAudit.Models;
using FntrAudit.Services.Auth;
using FntrAudit.Services.Personnel;
using FntrAudit.Viewmodels.Common;

namespace FntrAudit.Viewmodels
{
    public class AdminPersonnelViewModel : INotifyPropertyChanged
    {
        private readonly IPersonnelService _personnelService;
        private readonly IUserSessionService _userSessionService;
      

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? AddRequested;
        public event Action<User>? EditRequested;

        public ObservableCollection<EntityRowViewModel> Rows { get; } = new();

        private EntityRowViewModel? _selectedRow;
        public EntityRowViewModel? SelectedRow
        {
            get => _selectedRow;
            set
            {
                if (_selectedRow == value) return;
                _selectedRow = value;
                OnPropertyChanged();

                _addCommand.RaiseCanExecuteChanged();
                _openCommand.RaiseCanExecuteChanged();
                _editCommand.RaiseCanExecuteChanged();
                _deleteCommand.RaiseCanExecuteChanged();
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

        private readonly RelayCommand _editCommand;
        public ICommand EditCommand => _editCommand;

        private readonly RelayCommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand;

        public AdminPersonnelViewModel(
            IPersonnelService personnelService,
            IUserSessionService userSessionService)
        {
            _personnelService = personnelService;
            _userSessionService = userSessionService;

            _addCommand = new RelayCommand(Add, () => !IsBusy);
            _openCommand = new RelayCommand(Open, () => !IsBusy && SelectedRow != null);
            _editCommand = new RelayCommand(Edit, () => !IsBusy && SelectedRow != null);
            _deleteCommand = new RelayCommand(Delete, () => !IsBusy && SelectedRow != null);

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;
                Rows.Clear();

                var currentUser = _userSessionService.CurrentUser;
                if (currentUser?.idSociete == null)
                {
                    ErrorMessage = "Aucune société utilisateur n'est disponible.";
                    return;
                }

                var users = await _personnelService.GetUsersBySocieteAsync(currentUser.idSociete.Value);

                foreach (var user in users)
                {
                    string fullName = $"{user.nom} {user.prenom}".Trim();
                    if (string.IsNullOrWhiteSpace(fullName))
                        fullName = user.userName ?? user.email ?? user.mail ?? "Utilisateur";

                    string subtitle = user.email ?? user.mail ?? user.identifiant;

                    Rows.Add(new EntityRowViewModel(fullName, user, subtitle));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
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
            if (SelectedRow?.Item is not User user)
                return;

            // navigation détail utilisateur
        }

        private void Edit()
        {
            if (SelectedRow?.Item is not User user)
                return;

            EditRequested?.Invoke(user);
        }

        private void Delete()
        {
            if (SelectedRow?.Item is not User user)
                return;

            string fullName = $"{user.nom} {user.prenom}".Trim();
            if (string.IsNullOrWhiteSpace(fullName))
                fullName = user.email ?? user.mail ?? "cet utilisateur";

            var result = System.Windows.MessageBox.Show(
                $"Voulez-vous vraiment supprimer {fullName} ?",
                "Confirmation de suppression",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result != System.Windows.MessageBoxResult.Yes)
                return;

            _ = DeleteAsync(user);
        }

        public async Task ReloadAsync()
        {
            await LoadAsync();
        }

        private async Task DeleteAsync(User user)
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;

                await _personnelService.DeleteUserAsync(user.id);

                var rowToRemove = Rows.FirstOrDefault(r => (r.Item as User)?.id == user.id);
                if (rowToRemove != null)
                    Rows.Remove(rowToRemove);

                if (SelectedRow == rowToRemove)
                    SelectedRow = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la suppression : {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }




        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
