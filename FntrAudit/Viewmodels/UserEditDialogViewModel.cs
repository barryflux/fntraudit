using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using FntrAudit.Models;

namespace FntrAudit.Viewmodels
{
    public class UserEditDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly bool _isEditMode;
        private readonly User? _sourceUser;

        public string DialogTitle => _isEditMode ? "Modifier un utilisateur" : "Ajouter un utilisateur";

        public ObservableCollection<RoleOptionViewModel> Roles { get; } = new()
        {
            new RoleOptionViewModel(1, "Assistant juriste"),
            new RoleOptionViewModel(2, "Juriste")
        };

        private string? _nom;
        public string? Nom
        {
            get => _nom;
            set
            {
                if (_nom == value) return;
                _nom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValid));
                ClearErrorMessage();
            }
        }

        private string? _prenom;
        public string? Prenom
        {
            get => _prenom;
            set
            {
                if (_prenom == value) return;
                _prenom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValid));
                ClearErrorMessage();
            }
        }

        private string? _mail;
        public string? Mail
        {
            get => _mail;
            set
            {
                if (_mail == value) return;
                _mail = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValid));
                ClearErrorMessage();
            }
        }

        private string? _titre1;
        public string? Titre1
        {
            get => _titre1;
            set
            {
                if (_titre1 == value) return;
                _titre1 = value;
                OnPropertyChanged();
            }
        }

        private string? _titre2;
        public string? Titre2
        {
            get => _titre2;
            set
            {
                if (_titre2 == value) return;
                _titre2 = value;
                OnPropertyChanged();
            }
        }

        private string? _titre3;
        public string? Titre3
        {
            get => _titre3;
            set
            {
                if (_titre3 == value) return;
                _titre3 = value;
                OnPropertyChanged();
            }
        }

        private string? _titre4;
        public string? Titre4
        {
            get => _titre4;
            set
            {
                if (_titre4 == value) return;
                _titre4 = value;
                OnPropertyChanged();
            }
        }

        private RoleOptionViewModel? _selectedRole;
        public RoleOptionViewModel? SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (_selectedRole == value) return;
                _selectedRole = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValid));
                ClearErrorMessage();
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

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(Nom) &&
            !string.IsNullOrWhiteSpace(Prenom) &&
            SelectedRole != null &&
            !string.IsNullOrWhiteSpace(Mail) &&
            IsValidEmail(Mail);

        public UserEditDialogViewModel()
        {
            _isEditMode = false;
        }

        public UserEditDialogViewModel(User user)
        {
            _isEditMode = true;
            _sourceUser = user;

            Nom = user.nom;
            Prenom = user.prenom;
            Mail = user.email ?? user.mail;
            Titre1 = user.titre1;
            Titre2 = user.titre2;
            Titre3 = user.titre3;
            Titre4 = user.titre4;

            if (user.role.HasValue)
            {
                SelectedRole = Roles.FirstOrDefault(x => x.Value == user.role.Value);
            }

            OnPropertyChanged(nameof(IsValid));
        }

        public bool Validate()
        {
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Nom))
            {
                ErrorMessage = "Le nom est obligatoire.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Prenom))
            {
                ErrorMessage = "Le prénom est obligatoire.";
                return false;
            }

            if (SelectedRole == null)
            {
                ErrorMessage = "Le rôle est obligatoire.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Mail))
            {
                ErrorMessage = "Le mail est obligatoire.";
                return false;
            }

            if (!IsValidEmail(Mail))
            {
                ErrorMessage = "Le format du mail est invalide.";
                return false;
            }

            return true;
        }

        public User BuildUser(int societeId)
        {
            var user = _sourceUser ?? new User();

            user.idSociete = societeId;
            user.nom = Nom?.Trim();
            user.prenom = Prenom?.Trim();
            user.role = SelectedRole?.Value;
            user.mail = Mail?.Trim();
            user.email = Mail?.Trim();
            user.titre1 = Titre1?.Trim();
            user.titre2 = Titre2?.Trim();
            user.titre3 = Titre3?.Trim();
            user.titre4 = Titre4?.Trim();

            return user;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void ClearErrorMessage()
        {
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
                ErrorMessage = null;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RoleOptionViewModel
    {
        public int Value { get; }
        public string Label { get; }

        public RoleOptionViewModel(int value, string label)
        {
            Value = value;
            Label = label;
        }
    }
}