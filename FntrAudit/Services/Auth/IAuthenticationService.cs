using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Services.Auth
{
    public interface IAuthenticationService
    {
        Task<AuthResult> LoginAsync(string? email, string? password, CancellationToken cancellationToken = default);
    }
}
