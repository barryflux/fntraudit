using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Models;

namespace FntrAudit.Services.Auth
{
    public interface IUserSessionService
    {
        User? CurrentUser { get; }
        void SetCurrentUser(User user);
        void Clear();
    }
}
