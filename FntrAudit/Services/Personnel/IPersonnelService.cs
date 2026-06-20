using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Models;

namespace FntrAudit.Services.Personnel
{
    public interface IPersonnelService
    {
        Task<List<User>> GetUsersBySocieteAsync(int societeId, CancellationToken cancellationToken = default);
        Task AddUserAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
