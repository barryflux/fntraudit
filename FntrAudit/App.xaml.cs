using System.Configuration;
using System.Data;
using System.Windows;
using FntrAudit.Data;
using FntrAudit.Helpers;
using FntrAudit.Services.Activites;
using FntrAudit.Services.Auth;
using FntrAudit.Services.Clients;
using FntrAudit.Services.Personnel;
using FntrAudit.Services.Settings;
using FntrAudit.Viewmodels;
using FntrAudit.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SQLitePCL;

namespace FntrAudit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; } = null!;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    string dbPath = @"C:\refonte\TransAuditDocument\DB\TransAudit.db";

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite($"Data Source={dbPath}"));

                    services.AddTransient<IAuthenticationService, AuthenticationService>();
                    services.AddSingleton<IUserSettingsService, UserSettingsService>();
                    services.AddSingleton<IUserSessionService, UserSessionService>();
                    services.AddTransient<IPersonnelService, PersonnelService>();
                    services.AddTransient<IClientService, ClientService>();
                    services.AddTransient<IActivityService, ActivityService>();
                    services.AddTransient<IAuthHelpers, AuthHelper>();

                    services.AddTransient<AuthentificationViewModel>();
                    services.AddTransient<MainMenuViewModel>();
                    services.AddTransient<NewAuditView>();
                    services.AddTransient<ResumeAuditView>();
                    services.AddTransient<AdminPersonnelViewModel>();
                    services.AddTransient<AdminAuditViewModel>();
                    services.AddTransient<UserEditDialog>();
                    services.AddTransient<UserEditDialogViewModel>();
                    services.AddTransient<CreateClientViewModel>();
                    services.AddTransient<ClientEditDialog>();

                    services.AddTransient<AdminPersonnelView>();
                    services.AddTransient<AdminAuditView>();
                    services.AddTransient<AuthentView>();
                    services.AddTransient<MainMenu>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();

            var loginWindow = AppHost.Services.GetRequiredService<AuthentView>();
            MainWindow = loginWindow;
            loginWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            AppHost.Dispose();
            base.OnExit(e);
        }
    }

}
