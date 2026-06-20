using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FntrAudit.Helpers;
using FntrAudit.Models;
using FntrAudit.Services.Activites;
using FntrAudit.Services.Auth;
using FntrAudit.Viewmodels.Common;

namespace FntrAudit.Viewmodels
{
    public class CreateClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<bool>? RequestClose;
        

        private readonly Client? _sourceClient;
        private readonly IActivityService _activityService;

        public bool IsEditMode => _sourceClient != null;
        public string DialogTitle => IsEditMode ? "Modifier un client" : "Créer un client";
        public string ModeLabel => IsEditMode ? "Modification" : "Création";

        public ObservableCollection<ClientActivityRowViewModel> Activities { get; } = new();
        public ObservableCollection<EntityRowViewModel> ActivityItems { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddActivityCommand { get; }
        public ICommand PickLogoCommand { get; }
        public ICommand RemoveLogoCommand { get; }
        public ICommand EditActivityCommand { get; }
        public ICommand DeleteActivityCommand { get; }

        public CreateClientViewModel(IActivityService activityService)
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            DeleteCommand = new RelayCommand(Delete, () => IsEditMode);
            AddActivityCommand = new RelayCommand(AddActivity);
            PickLogoCommand = new RelayCommand(PickLogo);
            RemoveLogoCommand = new RelayCommand(RemoveLogo);         
            EditActivityCommand = new RelayCommand<EntityRowViewModel>(EditActivity);
            DeleteActivityCommand = new RelayCommand<EntityRowViewModel>(DeleteActivity);
            _activityService = activityService;

            _ = LoadActivitiesAsync();
        }

        public CreateClientViewModel(Client client, IActivityService activityService) : this(activityService)
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

        private string? _intitule;
        public string? Intitule
        {
            get => _intitule;
            set { _intitule = value; OnPropertyChanged(); }
        }

        private string? _personneSollicitante;
        public string? PersonneSollicitante
        {
            get => _personneSollicitante;
            set { _personneSollicitante = value; OnPropertyChanged(); }
        }

        private string? _personneInterrogee;
        public string? PersonneInterrogee
        {
            get => _personneInterrogee;
            set { _personneInterrogee = value; OnPropertyChanged(); }
        }

        private string? _fonction;
        public string? Fonction
        {
            get => _fonction;
            set { _fonction = value; OnPropertyChanged(); }
        }

        private string? _statut;
        public string? Statut
        {
            get => _statut;
            set { _statut = value; OnPropertyChanged(); }
        }

        private string? _capital;
        public string? Capital
        {
            get => _capital;
            set { _capital = value; OnPropertyChanged(); }
        }

        private string? _raisonSociale;
        public string? RaisonSociale
        {
            get => _raisonSociale;
            set { _raisonSociale = value; OnPropertyChanged(); }
        }

        private string? _siret;
        public string? Siret
        {
            get => _siret;
            set { _siret = value; OnPropertyChanged(); }
        }

        private string? _adresse;
        public string? Adresse
        {
            get => _adresse;
            set { _adresse = value; OnPropertyChanged(); }
        }

        private string? _naf;
        public string? Naf
        {
            get => _naf;
            set { _naf = value; OnPropertyChanged(); }
        }

        private string? _formeJuridique;
        public string? FormeJuridique
        {
            get => _formeJuridique;
            set { _formeJuridique = value; OnPropertyChanged(); }
        }

        private string? _caAnnuel;
        public string? CaAnnuel
        {
            get => _caAnnuel;
            set { _caAnnuel = value; OnPropertyChanged(); }
        }

        private string? _historique;
        public string? Historique
        {
            get => _historique;
            set { _historique = value; OnPropertyChanged(); }
        }

        private string? _etabSecondaire;
        public string? EtabSecondaire
        {
            get => _etabSecondaire;
            set { _etabSecondaire = value; OnPropertyChanged(); }
        }

        private string? _effectif;
        public string? Effectif
        {
            get => _effectif;
            set { _effectif = value; OnPropertyChanged(); }
        }

        private string? _nombreLicence;
        public string? NombreLicence
        {
            get => _nombreLicence;
            set { _nombreLicence = value; OnPropertyChanged(); }
        }

        private string? _nbreVehiculeMoteur;
        public string? NbreVehiculeMoteur
        {
            get => _nbreVehiculeMoteur;
            set { _nbreVehiculeMoteur = value; OnPropertyChanged(); }
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private bool _has1SalOrMore;
        public bool Has1SalOrMore
        {
            get => _has1SalOrMore;
            set { _has1SalOrMore = value; OnPropertyChanged(); }
        }

        private bool _has11SalOrMore;
        public bool Has11SalOrMore
        {
            get => _has11SalOrMore;
            set { _has11SalOrMore = value; OnPropertyChanged(); }
        }

        private bool _has50SalOrMore;
        public bool Has50SalOrMore
        {
            get => _has50SalOrMore;
            set { _has50SalOrMore = value; OnPropertyChanged(); }
        }

        private bool _has300SalOrMore;
        public bool Has300SalOrMore
        {
            get => _has300SalOrMore;
            set { _has300SalOrMore = value; OnPropertyChanged(); }
        }

        private bool _has1000SalOrMore;
        public bool Has1000SalOrMore
        {
            get => _has1000SalOrMore;
            set { _has1000SalOrMore = value; OnPropertyChanged(); }
        }

        private bool _pvCarence;
        public bool PvCarence
        {
            get => _pvCarence;
            set { _pvCarence = value; OnPropertyChanged(); }
        }

        private bool _cse;
        public bool Cse
        {
            get => _cse;
            set { _cse = value; OnPropertyChanged(); }
        }

        private bool _isVoyageur;
        public bool IsVoyageur
        {
            get => _isVoyageur;
            set { _isVoyageur = value; OnPropertyChanged(); }
        }

        private bool _isTransport;
        public bool IsTransport
        {
            get => _isTransport;
            set { _isTransport = value; OnPropertyChanged(); }
        }

        private BitmapImage? _logoPreview;
        public BitmapImage? LogoPreview
        {
            get => _logoPreview;
            set { _logoPreview = value; OnPropertyChanged(); }
        }

        private byte[]? _logoBytes;
        public byte[]? LogoBytes
        {
            get => _logoBytes;
            set { _logoBytes = value; OnPropertyChanged(); }
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public bool Validate()
        {
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Intitule))
            {
                ErrorMessage = "Le nom de la société est obligatoire.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
            {
                ErrorMessage = "Le format de l'email est invalide.";
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

            return client;
        }

        private void Save()
        {
            if (!Validate())
                return;

            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        private void Delete()
        {
            RequestClose?.Invoke(true);
        }

        private void AddActivity()
        {
            Activities.Add(new ClientActivityRowViewModel());
        }

        public void DeleteActivity(ClientActivityRowViewModel? activity)
        {
            if (activity == null)
                return;

            Activities.Remove(activity);
        }

        private void PickLogo()
        {
            // à brancher plus tard avec un OpenFileDialog
        }

        private void RemoveLogo()
        {
            LogoBytes = null;
            LogoPreview = null;
        }
        

        private void EditActivity(EntityRowViewModel? activity)
        {
            if (activity == null)
                return;

            // ouvrir ta modal d'édition plus tard
        }

        private void DeleteActivity(EntityRowViewModel? activity)
        {
            if (activity == null)
                return;

            ActivityItems.Remove(activity);
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
                    Item = activity.id,
                    Title = activity.intitule,
                    Subtitle = ""
                });
            }
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