using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FntrAudit.Helpers;
using FntrAudit.Models;
using FntrAudit.Services.Activites;
using FntrAudit.Services.Clients;
using FntrAudit.Viewmodels.Common;
using FntrAudit.Views;
using Microsoft.Win32;

namespace FntrAudit.Viewmodels
{
    public class CreateClientViewModel : INotifyPropertyChanged
    {
        private const string ErrorBrush = "#C62828";
        private const string DefaultBorderBrush = "#D0D0D0";

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<bool>? RequestClose;

        private readonly Client? _sourceClient;
        private readonly IActivityService _activityService;
        private readonly IClientService _clientService;
        private readonly int _societeId;
        private readonly RelayCommand<EntityRowViewModel> _activitySelectionChangedCommand;
        private readonly RelayCommand _saveCommand;
        private readonly RelayCommand _deleteCommand;
        private bool _validationRequested;

        public bool IsEditMode => _sourceClient != null;
        public string DialogTitle => IsEditMode ? "Modifier un client" : "Créer un client";
        public string ModeLabel => IsEditMode ? "Modification" : "Création";

        public ObservableCollection<ClientActivityRowViewModel> Activities { get; } = new();
        public ObservableCollection<EntityRowViewModel> ActivityItems { get; } = new();

        public ICommand SaveCommand => _saveCommand;
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand => _deleteCommand;
        public ICommand AddActivityCommand { get; }
        public ICommand PickLogoCommand { get; }
        public ICommand RemoveLogoCommand { get; }
        public ICommand EditActivityCommand { get; }
        public ICommand DeleteActivityCommand { get; }

        public CreateClientViewModel(
            IActivityService activityService,
            IClientService clientService,
            int societeId)
        {
            _activityService = activityService;
            _clientService = clientService;
            _societeId = societeId;
            _activitySelectionChangedCommand = new RelayCommand<EntityRowViewModel>(ActivitySelectionChanged);

            _saveCommand = new RelayCommand(Save, () => !IsSaving);
            _deleteCommand = new RelayCommand(Delete, () => IsEditMode && !IsSaving);
            CancelCommand = new RelayCommand(Cancel, () => !IsSaving);
            AddActivityCommand = new RelayCommand(AddActivity, () => !IsSaving);
            PickLogoCommand = new RelayCommand(PickLogo, () => !IsSaving);
            RemoveLogoCommand = new RelayCommand(RemoveLogo, () => !IsSaving);
            EditActivityCommand = new RelayCommand<EntityRowViewModel>(EditActivity);
            DeleteActivityCommand = new RelayCommand<EntityRowViewModel>(DeleteActivity);

            _ = LoadActivitiesAsync();
        }

        public CreateClientViewModel(
            Client client,
            IActivityService activityService,
            IClientService clientService,
            int societeId) : this(activityService, clientService, societeId)
        {
            _sourceClient = client;

            Intitule = client.intitule;
            PersonneSollicitante = client.personneSollicitante;
            PersonneInterrogee = client.personneInterrogee;
            Fonction = client.fonction;
            Statut = client.statut;
            Capital = client.capital;
            RaisonSociale = client.raisonSociale;
            Siret = client.siret;
            Adresse = client.adresse;
            Naf = client.naf;
            FormeJuridique = client.formeJuridique;
            CaAnnuel = client.caAnnuel;
            Historique = client.historique;
            EtabSecondaire = client.etabSecondaire;
            Effectif = client.effectif;
            NombreLicence = client.nombreLicence;
            NbreVehiculeMoteur = client.nbreVehiculeMoteur;
            PvCarence = client.pvCarence;
            Cse = client.cse;
            IsVoyageur = client.isVoyageur;
            IsTransport = client.isTransport;
            Email = client.email;

            Has1SalOrMore = client.has1SalOrMore;
            Has11SalOrMore = client.has11SalOrMore;
            Has50SalOrMore = client.has50SalOrMore;
            Has300SalOrMore = client.has300SalOrMore;
            Has1000SalOrMore = client.has1000SalOrMore;

            if (client.picture != null && client.picture.Length > 0)
            {
                LogoBytes = client.picture;
                LogoPreview = ByteArrayToImage(client.picture);
            }

            Activities.Clear();
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (_isSaving == value) return;
                _isSaving = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SaveButtonText));
                _saveCommand.RaiseCanExecuteChanged();
                _deleteCommand.RaiseCanExecuteChanged();
            }
        }

        public string SaveButtonText => IsSaving ? "Enregistrement..." : "Enregistrer";

        private string? _intitule;
        public string? Intitule
        {
            get => _intitule;
            set
            {
                _intitule = value;
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private string? _personneSollicitante;
        public string? PersonneSollicitante { get => _personneSollicitante; set { _personneSollicitante = value; OnPropertyChanged(); } }

        private string? _personneInterrogee;
        public string? PersonneInterrogee
        {
            get => _personneInterrogee;
            set
            {
                _personneInterrogee = value;
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private string? _fonction;
        public string? Fonction
        {
            get => _fonction;
            set
            {
                _fonction = value;
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private string? _statut;
        public string? Statut { get => _statut; set { _statut = value; OnPropertyChanged(); } }

        private string? _capital;
        public string? Capital { get => _capital; set { _capital = value; OnPropertyChanged(); } }

        private string? _raisonSociale;
        public string? RaisonSociale { get => _raisonSociale; set { _raisonSociale = value; OnPropertyChanged(); } }

        private string? _siret;
        public string? Siret
        {
            get => _siret;
            set
            {
                _siret = value;
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private string? _adresse;
        public string? Adresse { get => _adresse; set { _adresse = value; OnPropertyChanged(); } }

        private string? _naf;
        public string? Naf { get => _naf; set { _naf = value; OnPropertyChanged(); } }

        private string? _formeJuridique;
        public string? FormeJuridique { get => _formeJuridique; set { _formeJuridique = value; OnPropertyChanged(); } }

        private string? _caAnnuel;
        public string? CaAnnuel { get => _caAnnuel; set { _caAnnuel = value; OnPropertyChanged(); } }

        private string? _historique;
        public string? Historique { get => _historique; set { _historique = value; OnPropertyChanged(); } }

        private string? _etabSecondaire;
        public string? EtabSecondaire { get => _etabSecondaire; set { _etabSecondaire = value; OnPropertyChanged(); } }

        private string? _effectif;
        public string? Effectif { get => _effectif; set { _effectif = value; OnPropertyChanged(); } }

        private string? _nombreLicence;
        public string? NombreLicence { get => _nombreLicence; set { _nombreLicence = value; OnPropertyChanged(); } }

        private string? _nbreVehiculeMoteur;
        public string? NbreVehiculeMoteur { get => _nbreVehiculeMoteur; set { _nbreVehiculeMoteur = value; OnPropertyChanged(); } }

        private string? _email;
        public string? Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private bool _has1SalOrMore;
        public bool Has1SalOrMore { get => _has1SalOrMore; set { SetEffectifRange(nameof(Has1SalOrMore), value); } }

        private bool _has11SalOrMore;
        public bool Has11SalOrMore { get => _has11SalOrMore; set { SetEffectifRange(nameof(Has11SalOrMore), value); } }

        private bool _has50SalOrMore;
        public bool Has50SalOrMore { get => _has50SalOrMore; set { SetEffectifRange(nameof(Has50SalOrMore), value); } }

        private bool _has300SalOrMore;
        public bool Has300SalOrMore { get => _has300SalOrMore; set { SetEffectifRange(nameof(Has300SalOrMore), value); } }

        private bool _has1000SalOrMore;
        public bool Has1000SalOrMore { get => _has1000SalOrMore; set { SetEffectifRange(nameof(Has1000SalOrMore), value); } }

        private bool _pvCarence;
        public bool PvCarence
        {
            get => _pvCarence;
            set
            {
                if (_pvCarence == value) return;
                _pvCarence = value;
                if (value && _cse)
                {
                    _cse = false;
                    OnPropertyChanged(nameof(Cse));
                }
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private bool _cse;
        public bool Cse
        {
            get => _cse;
            set
            {
                if (_cse == value) return;
                _cse = value;
                if (value && _pvCarence)
                {
                    _pvCarence = false;
                    OnPropertyChanged(nameof(PvCarence));
                }
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private bool _isVoyageur;
        public bool IsVoyageur
        {
            get => _isVoyageur;
            set
            {
                if (_isVoyageur == value) return;
                _isVoyageur = value;
                if (value && _isTransport)
                {
                    _isTransport = false;
                    OnPropertyChanged(nameof(IsTransport));
                }
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private bool _isTransport;
        public bool IsTransport
        {
            get => _isTransport;
            set
            {
                if (_isTransport == value) return;
                _isTransport = value;
                if (value && _isVoyageur)
                {
                    _isVoyageur = false;
                    OnPropertyChanged(nameof(IsVoyageur));
                }
                OnPropertyChanged();
                NotifyValidationPropertiesChanged();
            }
        }

        private BitmapImage? _logoPreview;
        public BitmapImage? LogoPreview { get => _logoPreview; set { _logoPreview = value; OnPropertyChanged(); } }

        private byte[]? _logoBytes;
        public byte[]? LogoBytes { get => _logoBytes; set { _logoBytes = value; OnPropertyChanged(); } }

        private string? _errorMessage;
        public string? ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public bool IsIntituleInvalid => _validationRequested && string.IsNullOrWhiteSpace(Intitule);
        public bool IsEmailInvalid => _validationRequested && (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email));
        public bool IsSiretInvalid => _validationRequested && string.IsNullOrWhiteSpace(Siret);
        public bool IsPersonneInterrogeeInvalid => _validationRequested && string.IsNullOrWhiteSpace(PersonneInterrogee);
        public bool IsFonctionInvalid => _validationRequested && string.IsNullOrWhiteSpace(Fonction);
        public bool IsEffectifRangeInvalid => _validationRequested && !HasAnyEffectifRange;
        public bool IsCsePvInvalid => _validationRequested && !Cse && !PvCarence;
        public bool IsTransportTypeInvalid => _validationRequested && !IsVoyageur && !IsTransport;

        public string IntituleBorderBrush => IsIntituleInvalid ? ErrorBrush : DefaultBorderBrush;
        public string EmailBorderBrush => IsEmailInvalid ? ErrorBrush : DefaultBorderBrush;
        public string SiretBorderBrush => IsSiretInvalid ? ErrorBrush : DefaultBorderBrush;
        public string PersonneInterrogeeBorderBrush => IsPersonneInterrogeeInvalid ? ErrorBrush : DefaultBorderBrush;
        public string FonctionBorderBrush => IsFonctionInvalid ? ErrorBrush : DefaultBorderBrush;
        public string EffectifRangeBorderBrush => IsEffectifRangeInvalid ? ErrorBrush : DefaultBorderBrush;
        public string CsePvBorderBrush => IsCsePvInvalid ? ErrorBrush : DefaultBorderBrush;
        public string TransportTypeBorderBrush => IsTransportTypeInvalid ? ErrorBrush : DefaultBorderBrush;

        private bool HasAnyEffectifRange => Has1SalOrMore || Has11SalOrMore || Has50SalOrMore || Has300SalOrMore || Has1000SalOrMore;

        public bool Validate()
        {
            _validationRequested = true;
            NotifyValidationPropertiesChanged();
            ErrorMessage = null;

            if (IsIntituleInvalid || IsEmailInvalid || IsSiretInvalid || IsPersonneInterrogeeInvalid || IsFonctionInvalid || IsEffectifRangeInvalid || IsCsePvInvalid || IsTransportTypeInvalid)
            {
                ErrorMessage = "Veuillez renseigner tous les champs obligatoires avant d'enregistrer.";
                return false;
            }

            return true;
        }

        public Client BuildClient(int societeId)
        {
            var client = _sourceClient ?? new Client();

            client.societeUser_Id = societeId;
            client.intitule = Intitule?.Trim();
            client.personneSollicitante = PersonneSollicitante?.Trim();
            client.personneInterrogee = PersonneInterrogee?.Trim();
            client.fonction = Fonction?.Trim();
            client.statut = Statut?.Trim();
            client.capital = Capital?.Trim();
            client.raisonSociale = RaisonSociale?.Trim();
            client.siret = Siret?.Trim();
            client.adresse = Adresse?.Trim();
            client.naf = Naf?.Trim();
            client.formeJuridique = FormeJuridique?.Trim();
            client.caAnnuel = CaAnnuel?.Trim();
            client.historique = Historique?.Trim();
            client.etabSecondaire = EtabSecondaire?.Trim();
            client.effectif = Effectif?.Trim();
            client.nombreLicence = NombreLicence?.Trim();
            client.nbreVehiculeMoteur = NbreVehiculeMoteur?.Trim();
            client.pvCarence = PvCarence;
            client.cse = Cse;
            client.isVoyageur = IsVoyageur;
            client.isTransport = IsTransport;
            client.email = Email?.Trim();
            client.has1SalOrMore = Has1SalOrMore;
            client.has11SalOrMore = Has11SalOrMore;
            client.has50SalOrMore = Has50SalOrMore;
            client.has300SalOrMore = Has300SalOrMore;
            client.has1000SalOrMore = Has1000SalOrMore;
            client.picture = LogoBytes;
            client.activite = BuildActivitiesJson();

            return client;
        }

        private async void Save()
        {
            if (!Validate())
                return;

            try
            {
                IsSaving = true;
                ErrorMessage = null;
                var client = BuildClient(_societeId);

                if (IsEditMode)
                    await _clientService.UpdateClientAsync(client);
                else
                    await _clientService.AddClientAsync(client);

                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la sauvegarde du client : {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void Cancel() => RequestClose?.Invoke(false);

        private async void Delete()
        {
            if (!IsEditMode || _sourceClient == null)
                return;

            var confirmed = DeleteConfirmationDialog.Confirm(
                Application.Current.MainWindow,
                $"Supprimer le client '{Intitule ?? RaisonSociale ?? "Client"}' ?");

            if (!confirmed)
                return;

            try
            {
                IsSaving = true;
                ErrorMessage = null;
                await _clientService.DeleteClientAsync(_sourceClient.id);
                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la suppression du client : {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async void AddActivity()
        {
            try
            {
                ErrorMessage = null;
                var dialogVm = new ActivityEditDialogViewModel();
                var dialog = new ActivityEditDialog(dialogVm)
                {
                    Owner = Application.Current.MainWindow
                };

                if (dialog.ShowDialog() != true)
                    return;

                await _activityService.AddActivityAsync(dialogVm.BuildActivity());
                await LoadActivitiesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de l'ajout de l'activité : {ex.Message}";
            }
        }

        public void DeleteActivity(ClientActivityRowViewModel? activity)
        {
            if (activity == null)
                return;

            Activities.Remove(activity);
        }

        private void PickLogo()
        {
            try
            {
                ErrorMessage = null;

                var dialog = new OpenFileDialog
                {
                    Title = "Choisir une image",
                    Filter = "Images (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Tous les fichiers (*.*)|*.*",
                    CheckFileExists = true,
                    Multiselect = false
                };

                if (dialog.ShowDialog() != true)
                    return;

                var bytes = File.ReadAllBytes(dialog.FileName);
                LogoBytes = bytes;
                LogoPreview = ByteArrayToImage(bytes);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la sélection du logo : {ex.Message}";
            }
        }

        private void RemoveLogo()
        {
            LogoBytes = null;
            LogoPreview = null;
        }

        private async void EditActivity(EntityRowViewModel? row)
        {
            if (row?.Item is not Activity activity)
                return;

            try
            {
                ErrorMessage = null;
                var dialogVm = new ActivityEditDialogViewModel(activity);
                var dialog = new ActivityEditDialog(dialogVm)
                {
                    Owner = Application.Current.MainWindow
                };

                if (dialog.ShowDialog() != true)
                    return;

                await _activityService.UpdateActivityAsync(dialogVm.BuildActivity());
                await LoadActivitiesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la modification de l'activité : {ex.Message}";
            }
        }

        private async void DeleteActivity(EntityRowViewModel? row)
        {
            if (row?.Item is not Activity activity)
                return;

            bool confirmed = DeleteConfirmationDialog.Confirm(
                Application.Current.MainWindow,
                $"Supprimer l'activité '{activity.intitule}' ?");

            if (!confirmed)
                return;

            try
            {
                ErrorMessage = null;
                await _activityService.DeleteActivityAsync(activity.id);
                await LoadActivitiesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la suppression de l'activité : {ex.Message}";
            }
        }

        private async void ActivitySelectionChanged(EntityRowViewModel? row)
        {
            if (row?.Item is not Activity activity)
                return;

            try
            {
                ErrorMessage = null;
                await _activityService.UpdateActivityAsync(activity);
                row.Subtitle = activity.isOk ? "Sélectionnée" : string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la sauvegarde de la sélection : {ex.Message}";
                await LoadActivitiesAsync();
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        private async Task LoadActivitiesAsync()
        {
            ActivityItems.Clear();

            var activities = await _activityService.GetActivitiesAsync();

            foreach (var activity in activities)
            {
                ActivityItems.Add(new EntityRowViewModel
                {
                    Item = activity,
                    Title = activity.intitule ?? "Activité",
                    Subtitle = activity.isOk ? "Sélectionnée" : string.Empty,
                    ShowSelectionCheckBox = true,
                    SelectionChangedCommand = _activitySelectionChangedCommand
                });
            }
        }

        private string BuildActivitiesJson()
        {
            var selectedActivities = ActivityItems
                .Select(row => row.Item)
                .OfType<Activity>()
                .Where(activity => activity.isOk)
                .Select(activity => new
                {
                    id = activity.id,
                    intitule = activity.intitule,
                    isOk = activity.isOk,
                    auditId = activity.auditId
                })
                .ToList();

            return JsonSerializer.Serialize(selectedActivities);
        }

        private void SetEffectifRange(string selectedProperty, bool value)
        {
            if (value)
            {
                _has1SalOrMore = selectedProperty == nameof(Has1SalOrMore);
                _has11SalOrMore = selectedProperty == nameof(Has11SalOrMore);
                _has50SalOrMore = selectedProperty == nameof(Has50SalOrMore);
                _has300SalOrMore = selectedProperty == nameof(Has300SalOrMore);
                _has1000SalOrMore = selectedProperty == nameof(Has1000SalOrMore);
            }
            else
            {
                switch (selectedProperty)
                {
                    case nameof(Has1SalOrMore): _has1SalOrMore = false; break;
                    case nameof(Has11SalOrMore): _has11SalOrMore = false; break;
                    case nameof(Has50SalOrMore): _has50SalOrMore = false; break;
                    case nameof(Has300SalOrMore): _has300SalOrMore = false; break;
                    case nameof(Has1000SalOrMore): _has1000SalOrMore = false; break;
                }
            }

            OnPropertyChanged(nameof(Has1SalOrMore));
            OnPropertyChanged(nameof(Has11SalOrMore));
            OnPropertyChanged(nameof(Has50SalOrMore));
            OnPropertyChanged(nameof(Has300SalOrMore));
            OnPropertyChanged(nameof(Has1000SalOrMore));
            NotifyValidationPropertiesChanged();
        }

        private void NotifyValidationPropertiesChanged()
        {
            OnPropertyChanged(nameof(IsIntituleInvalid));
            OnPropertyChanged(nameof(IsEmailInvalid));
            OnPropertyChanged(nameof(IsSiretInvalid));
            OnPropertyChanged(nameof(IsPersonneInterrogeeInvalid));
            OnPropertyChanged(nameof(IsFonctionInvalid));
            OnPropertyChanged(nameof(IsEffectifRangeInvalid));
            OnPropertyChanged(nameof(IsCsePvInvalid));
            OnPropertyChanged(nameof(IsTransportTypeInvalid));
            OnPropertyChanged(nameof(IntituleBorderBrush));
            OnPropertyChanged(nameof(EmailBorderBrush));
            OnPropertyChanged(nameof(SiretBorderBrush));
            OnPropertyChanged(nameof(PersonneInterrogeeBorderBrush));
            OnPropertyChanged(nameof(FonctionBorderBrush));
            OnPropertyChanged(nameof(EffectifRangeBorderBrush));
            OnPropertyChanged(nameof(CsePvBorderBrush));
            OnPropertyChanged(nameof(TransportTypeBorderBrush));
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

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}