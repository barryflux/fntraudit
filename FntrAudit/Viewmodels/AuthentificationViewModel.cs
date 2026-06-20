using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FntrAudit.Helpers;
using FntrAudit.Services.Auth;
using FntrAudit.Services.Settings;

namespace FntrAudit.Viewmodels
{
    public class AuthentificationViewModel : INotifyPropertyChanged
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IUserSessionService _userSessionService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? LoginSucceeded;
        public event Action? ForgotPasswordRequested;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();

                _checkUserCommand.RaiseCanExecuteChanged();
                _passwordTextCommand.RaiseCanExecuteChanged();
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

        private string? _email;
        public string? Email
        {
            get => _email;
            set
            {
                if (_email == value) return;
                _email = value;
                OnPropertyChanged();
            }
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set
            {
                if (_password == value) return;
                _password = value;
                OnPropertyChanged();
            }
        }

        private bool _rememberEmail = true;
        public bool RememberEmail
        {
            get => _rememberEmail;
            set
            {
                if (_rememberEmail == value) return;
                _rememberEmail = value;
                OnPropertyChanged();
            }
        }

        private readonly RelayCommand _checkUserCommand;
        public ICommand CheckUser => _checkUserCommand;

        private readonly RelayCommand _passwordTextCommand;
        public ICommand PasswordText => _passwordTextCommand;

        public AuthentificationViewModel(
            IAuthenticationService authenticationService,
            IUserSettingsService userSettingsService,
            IUserSessionService userSessionService)
        {
            _authenticationService = authenticationService;
            _userSettingsService = userSettingsService;
            _userSessionService = userSessionService;

            Email = _userSettingsService.GetSavedEmail();

            _checkUserCommand = new RelayCommand(LoginAsync, () => !IsBusy);
            _passwordTextCommand = new RelayCommand(ForgotPassword, () => !IsBusy);
        }

        private async void LoginAsync()
        {
            IsBusy = true;
            ErrorMessage = null;

            try
            {
                var result = await _authenticationService.LoginAsync(Email, Password);

                if (!result.IsSuccess || result.User == null)
                {
                    ErrorMessage = result.ErrorMessage;
                    return;
                }

                _userSessionService.SetCurrentUser(result.User);

                if (RememberEmail)
                    _userSettingsService.SaveEmail(Email);
                else
                    _userSettingsService.ClearEmail();

                LoginSucceeded?.Invoke();
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

        private void ForgotPassword()
        {
            ForgotPasswordRequested?.Invoke();
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}