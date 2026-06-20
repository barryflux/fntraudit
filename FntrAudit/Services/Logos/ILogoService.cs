using FntrAudit.Models;

namespace FntrAudit.Services.Logos
{
    public interface ILogoService
    {
        Task<List<Logo>> GetAllLogoAsync(CancellationToken cancellationToken = default);
    }
}