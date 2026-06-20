using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Services.Settings
{
    public interface IUserSettingsService
    {
        string? GetSavedEmail();
        void SaveEmail(string? email);
        void ClearEmail();
    }
}
