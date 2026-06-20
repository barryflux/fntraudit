using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Data;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace FntrAudit.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _dbContext;

        public AuthenticationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AuthResult> LoginAsync(string? email, string? password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return AuthResult.Failure("Veuillez saisir votre adresse email.");

            if (string.IsNullOrWhiteSpace(password))
                return AuthResult.Failure("Veuillez saisir votre mot de passe.");

            string normalizedEmail = email.Trim().ToLowerInvariant();

            try
            {
                var user = await _dbContext.Aspnetusers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        u => u.mail != null && u.mail.ToLower() == normalizedEmail.ToLower(),
                        cancellationToken);

                if (user == null)
                    return AuthResult.Failure("Utilisateur introuvable.");

                // IMPORTANT :
                // Ici je compare en clair car tu as dit que le mot de passe est déjà en base.
                // Si plus tard il est hashé, il faudra remplacer cette logique.
                if (!string.Equals(user.pass, password, StringComparison.Ordinal))
                    return AuthResult.Failure("Mot de passe incorrect.");

                return AuthResult.Success(user);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Une erreur est survenue lors de la tentative de connexion.", ex);
            }
        }
    }
}
