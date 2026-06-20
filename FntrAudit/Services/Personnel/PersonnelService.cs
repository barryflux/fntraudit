using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Data;
using FntrAudit.Helpers;
using FntrAudit.Models;
using FntrAudit.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace FntrAudit.Services.Personnel
{
    public class PersonnelService : IPersonnelService
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserSessionService _userSessionService;
        private readonly IAuthHelpers  _authHelpers;

        public PersonnelService(AppDbContext dbContext, IUserSessionService userSessionService, IAuthHelpers authHelpers)
        {
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            _authHelpers = authHelpers;
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            user.societeUser_Id = _userSessionService?.CurrentUser?.idSociete;
            user.pass = await _authHelpers.GenerateRandomStringAsync();
            user.identifiant = $"{user.prenom} {user.nom}".Trim();
            _dbContext.Aspnetusers.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<User>> GetUsersBySocieteAsync(int societeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Aspnetusers
                .AsNoTracking()
                .Where(x => x.idSociete == societeId)
                .OrderBy(x => x.nom)
                .ThenBy(x => x.prenom)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var existingUser = await _dbContext.Aspnetusers.FirstOrDefaultAsync(x => x.id == user.id);

            if (existingUser == null)
                throw new Exception("Utilisateur introuvable.");

            existingUser.nom = user.nom;
            existingUser.prenom = user.prenom;
            existingUser.mail = user.mail;
            existingUser.email = user.email;
            existingUser.role = user.role;
            existingUser.titre1 = user.titre1;
            existingUser.titre2 = user.titre2;
            existingUser.titre3 = user.titre3;
            existingUser.titre4 = user.titre4;
            existingUser.societeUser_Id= user.societeUser_Id;
            existingUser.identifiant = $"{user.prenom} {user.nom}".Trim();

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Aspnetusers
                .FirstOrDefaultAsync(x => x.id == userId, cancellationToken);

            if (user == null)
                throw new InvalidOperationException("Utilisateur introuvable.");

            _dbContext.Aspnetusers.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
