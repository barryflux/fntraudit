using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Models;

namespace FntrAudit.Services.Activites
{
    public interface IActivityService
    {
        Task<List<Activity>> GetActivitiesAsync( CancellationToken cancellationToken = default);
        Task<Activity?> GetActivityByIdAsync(int activityId, CancellationToken cancellationToken = default);
        Task AddActivityAsync(Activity activity, CancellationToken cancellationToken = default);
        Task UpdateActivityAsync(Activity activity, CancellationToken cancellationToken = default);
        Task DeleteActivityAsync(int activityId, CancellationToken cancellationToken = default);
    }
}
