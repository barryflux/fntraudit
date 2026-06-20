using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FntrAudit.Viewmodels.Common
{
    public class EntityRowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private object? _item;
        public object? Item
        {
            get => _item;
            set
            {
                if (_item == value) return;
                _item = value;
                OnPropertyChanged();
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        private string? _subtitle;
        public string? Subtitle
        {
            get => _subtitle;
            set
            {
                if (_subtitle == value) return;
                _subtitle = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public EntityRowViewModel()
        {
        }

        public EntityRowViewModel(string title, object? item = null, string? subtitle = null)
        {
            Title = title;
            Item = item;
            Subtitle = subtitle;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}