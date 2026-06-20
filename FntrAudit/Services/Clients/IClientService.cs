using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FntrAudit.Models;

namespace FntrAudit.Services.Clients
{
    public interface IClientService
    {
        Task<List<Client>> GetClientsBySocieteAsync(int societeId, CancellationToken cancellationToken = default);
        Task<List<Client>> GetClientsWithAuditToResumeBySocieteAsync(int societeId, CancellationToken cancellationToken = default);
        Task<Client?> GetClientByIdAsync(int clientId, CancellationToken cancellationToken = default);
        Task AddClientAsync(Client client, CancellationToken cancellationToken = default);
        Task UpdateClientAsync(Client client, CancellationToken cancellationToken = default);
        Task DeleteClientAsync(int clientId, CancellationToken cancellationToken = default);
    }
}