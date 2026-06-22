using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FntrAudit.Data;
using FntrAudit.Models;
using Microsoft.EntityFrameworkCore;

namespace FntrAudit.Services.Clients
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _dbContext;

        public ClientService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Client>> GetClientsBySocieteAsync(int societeId, CancellationToken cancellationToken = default)
        {
            var clients = await _dbContext.Client
                .AsNoTracking()
                .Where(c => c.societeUser_Id == societeId)
                .ToListAsync(cancellationToken);

            return clients
                .OrderBy(c => c.intitule)
                .ToList();
        }

        public async Task<List<Client>> GetClientsWithAuditToResumeBySocieteAsync(int societeId, CancellationToken cancellationToken = default)
        {
            var clients = await _dbContext.Client
                .AsNoTracking()
                .Where(c => c.societeUser_Id == societeId)
                .Where(c => !string.IsNullOrWhiteSpace(c.auditEncours))
                .ToListAsync(cancellationToken);

            return clients
                .OrderBy(c => c.intitule)
                .ToList();
        }

        public async Task<Client?> GetClientByIdAsync(int clientId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Client
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.id == clientId, cancellationToken);
        }

        public async Task AddClientAsync(Client client, CancellationToken cancellationToken = default)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            ValidateClientForPersistence(client);

            await _dbContext.Client.AddAsync(client, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateClientAsync(Client client, CancellationToken cancellationToken = default)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            ValidateClientForPersistence(client);

            var existingClient = await _dbContext.Client
                .FirstOrDefaultAsync(c => c.id == client.id, cancellationToken);

            if (existingClient == null)
                throw new InvalidOperationException("Client introuvable.");

            existingClient.intitule = client.intitule;
            existingClient.personneSollicitante = client.personneSollicitante;
            existingClient.personneInterrogee = client.personneInterrogee;
            existingClient.fonction = client.fonction;
            existingClient.statut = client.statut;
            existingClient.capital = client.capital;
            existingClient.raisonSociale = client.raisonSociale;
            existingClient.siret = client.siret;
            existingClient.adresse = client.adresse;
            existingClient.naf = client.naf;
            existingClient.formeJuridique = client.formeJuridique;
            existingClient.caAnnuel = client.caAnnuel;
            existingClient.logo = client.logo;
            existingClient.historique = client.historique;
            existingClient.etabSecondaire = client.etabSecondaire;
            existingClient.idActivite = client.idActivite;
            existingClient.societeUser_Id = client.societeUser_Id;
            existingClient.auditEncours = client.auditEncours;
            existingClient.activite = client.activite;

            existingClient.has1SalOrMore = client.has1SalOrMore;
            existingClient.has11SalOrMore = client.has11SalOrMore;
            existingClient.has50SalOrMore = client.has50SalOrMore;
            existingClient.has300SalOrMore = client.has300SalOrMore;
            existingClient.has1000SalOrMore = client.has1000SalOrMore;
            existingClient.hasWarningRequiredDoc = client.hasWarningRequiredDoc;

            existingClient.effectif = client.effectif;
            existingClient.nombreLicence = client.nombreLicence;
            existingClient.nbreVehiculeMoteur = client.nbreVehiculeMoteur;

            existingClient.pvCarence = client.pvCarence;
            existingClient.cse = client.cse;
            existingClient.picture = client.picture;
            existingClient.isVoyageur = client.isVoyageur;
            existingClient.isTransport = client.isTransport;
            existingClient.email = client.email;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteClientAsync(int clientId, CancellationToken cancellationToken = default)
        {
            var existingClient = await _dbContext.Client
                .FirstOrDefaultAsync(c => c.id == clientId, cancellationToken);

            if (existingClient == null)
                throw new InvalidOperationException("Client introuvable.");

            _dbContext.Client.Remove(existingClient);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static void ValidateClientForPersistence(Client client)
        {
            if (client.societeUser_Id == null)
                throw new InvalidOperationException("Aucune société utilisateur n'est associée au client.");

            if (string.IsNullOrWhiteSpace(client.intitule))
                throw new InvalidOperationException("Le nom de la société est obligatoire.");

            if (string.IsNullOrWhiteSpace(client.email))
                throw new InvalidOperationException("L'email est obligatoire.");

            if (!IsValidEmail(client.email))
                throw new InvalidOperationException("Le format de l'email est invalide.");

            if (string.IsNullOrWhiteSpace(client.siret))
                throw new InvalidOperationException("Le SIRET est obligatoire.");

            if (string.IsNullOrWhiteSpace(client.personneInterrogee))
                throw new InvalidOperationException("La personne interrogée est obligatoire.");

            if (string.IsNullOrWhiteSpace(client.fonction))
                throw new InvalidOperationException("La fonction est obligatoire.");

            if (!HasOneEffectifRange(client))
                throw new InvalidOperationException("Une tranche d'effectif doit être sélectionnée.");

            if (!client.cse && !client.pvCarence)
                throw new InvalidOperationException("CSE ou PV de carence doit être sélectionné.");

            if (client.cse && client.pvCarence)
                throw new InvalidOperationException("CSE et PV de carence ne peuvent pas être sélectionnés ensemble.");

            if (!client.isVoyageur && !client.isTransport)
                throw new InvalidOperationException("Voyageur ou Marchandise / Transport doit être sélectionné.");

            if (client.isVoyageur && client.isTransport)
                throw new InvalidOperationException("Voyageur et Marchandise / Transport ne peuvent pas être sélectionnés ensemble.");
        }

        private static bool HasOneEffectifRange(Client client)
        {
            var selectedCount = 0;
            if (client.has1SalOrMore) selectedCount++;
            if (client.has11SalOrMore) selectedCount++;
            if (client.has50SalOrMore) selectedCount++;
            if (client.has300SalOrMore) selectedCount++;
            if (client.has1000SalOrMore) selectedCount++;
            return selectedCount == 1;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }
    }
}