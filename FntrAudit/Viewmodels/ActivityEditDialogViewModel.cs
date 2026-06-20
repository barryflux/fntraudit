using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FntrAudit.Helpers;
using FntrAudit.Models;

namespace FntrAudit.Viewmodels
{
    public class ActivityEditDialogViewModel : INotifyPropertyChanged
    {
        private readonly Activity? _sourceActivity;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<bool>? RequestClose;

        public string DialogTitle => IsEditMode ? "Modifier une activité" : "Ajouter une activité";
        public bool IsEditMode => _sourceActivity != null;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ActivityEditDialogViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        public ActivityEditDialogViewModel(Activity activity) : this()
        {
            _sourceActivity = activity;
            Intitule = activity.intitule;
            IsOk = activity.isOk;
        }

        private string? _intitule;
        public string? Intitule
        {
            get => _intitule;
            set
            {
                if (_intitule == value) return;
                _intitule = value;
                OnPropertyChanged();
            }
        }

        private bool _isOk;
        public bool IsOk
        {
            get => _isOk;
            set
            {
                if (_isOk == value) return;
                _isOk = value;
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

        public Activity BuildActivity()
        {
            var activity = _sourceActivity ?? new Activity();
            activity.intitule = Intitule?.Trim();
            activity.isOk = IsOk;

            return activity;
        }

        private void Save()
        {
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Intitule))
            {
                ErrorMessage = "L'intitulé de l'activité est obligatoire.";
                return;
            }

            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}