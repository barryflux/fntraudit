using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Data;
using FntrAudit.Models;
using FntrAudit.Services.LogoService;
using Microsoft.EntityFrameworkCore;

namespace FntrAudit.Services.LogoService
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

            return logos;
        }

    }
}
