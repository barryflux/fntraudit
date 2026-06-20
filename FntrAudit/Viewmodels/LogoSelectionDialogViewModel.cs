using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FntrAudit.Helpers;
using FntrAudit.Models;
using FntrAudit.Services.LogoService;

namespace FntrAudit.Viewmodels
{
    public class LogoSelectionDialogViewModel : INotifyPropertyChanged
    {
        private readonly ILogoService _logoService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<bool>? RequestClose;

        public ObservableCollection<LogoItemViewModel> Logos { get; } = new();

        public ICommand SelectLogoCommand { get; }
        public ICommand CancelCommand { get; }

        public Logo? SelectedLogo { get; private set; }

        public LogoSelectionDialogViewModel(ILogoService logoService)
        {
            _logoService = logoService;
            SelectLogoCommand = new RelayCommand<LogoItemViewModel>(SelectLogo);
            CancelCommand = new RelayCommand(Cancel);

            _ = LoadAsync();
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

        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;
                Logos.Clear();

                var logos = await _logoService.GetAllLogoAsync();

                foreach (var logo in logos)
                {
                    Logos.Add(new LogoItemViewModel(logo));
                }

                if (Logos.Count == 0)
                    ErrorMessage = "Aucun logo disponible.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des logos : {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SelectLogo(LogoItemViewModel? logoItem)
        {
            if (logoItem?.Logo == null)
                return;

            SelectedLogo = logoItem.Logo;
            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class LogoItemViewModel
    {
        public Logo Logo { get; }
        public string Title { get; }
        public BitmapImage? Preview { get; }

        public LogoItemViewModel(Logo logo)
        {
            Logo = logo;
            Title = string.IsNullOrWhiteSpace(logo.intitule) ? "Logo" : logo.intitule;
            Preview = logo.Logolo is { Length: > 0 } ? ByteArrayToImage(logo.Logolo) : null;
        }

        private static BitmapImage ByteArrayToImage(byte[] bytes)
        {
            using var stream = new System.IO.MemoryStream(bytes);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}