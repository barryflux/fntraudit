using FntrAudit.Data;
using FntrAudit.Models;
using Microsoft.EntityFrameworkCore;

namespace FntrAudit.Services.Logos
{
    public class LogoService : ILogoService
    {
        private readonly AppDbContext _dbContext;

        public LogoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Logo>> GetAllLogoAsync(CancellationToken cancellationToken = default)
        {
            var logos = await _dbContext.Logo
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return logos
                .OrderBy(logo => logo.intitule)
                .ToList();
        }
    }
}