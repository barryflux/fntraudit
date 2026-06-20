using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Models;

namespace FntrAudit.Services.LogoService
{
    public interface ILogoService
    {
        Task<List<Logo>> GetAllLogoAsync(CancellationToken cancellationToken = default);
    }
}
