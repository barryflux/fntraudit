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

namespace FntrAudit.Viewmodels.Navigation
{
    public class MenuItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title { get; }

        public ObservableCollection<MenuItemViewModel> Children { get; } = new();

        private bool _isExpandable;
        public bool IsExpandable
        {
            get => _isExpandable;
            set
            {
                if (_isExpandable == value) return;
                _isExpandable = value;
                OnPropertyChanged();
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        private readonly Func<object>? _contentFactory;
        public object? Content => _contentFactory?.Invoke();

        public ICommand? SelectCommand { get; }

        public MenuItemViewModel(string title)
        {
            Title = title;
        }

        public MenuItemViewModel(string title, Func<object> contentFactory, Action<MenuItemViewModel> onSelected)
        {
            Title = title;
            _contentFactory = contentFactory;
            SelectCommand = new RelayCommand(() => onSelected(this));
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
