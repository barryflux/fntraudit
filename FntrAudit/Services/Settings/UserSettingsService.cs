using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FntrAudit.Services.Settings
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly string _settingsFilePath;

        public UserSettingsService()
        {
            string appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FntrAudit");

            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            _settingsFilePath = Path.Combine(appFolder, "userSettings.json");
        }

        public string? GetSavedEmail()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                    return null;

                string json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<UserSettingsDto>(json);

                return settings?.SavedEmail;
            }
            catch
            {
                return null;
            }
        }

        public void SaveEmail(string? email)
        {
            try
            {
                var settings = new UserSettingsDto
                {
                    SavedEmail = email?.Trim()
                };

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_settingsFilePath, json);
            }
            catch
            {
                // volontairement silencieux pour ne pas bloquer le login
            }
        }

        public void ClearEmail()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                    File.Delete(_settingsFilePath);
            }
            catch
            {
                // ignore
            }
        }

        private class UserSettingsDto
        {
            public string? SavedEmail { get; set; }
        }
    }
}
