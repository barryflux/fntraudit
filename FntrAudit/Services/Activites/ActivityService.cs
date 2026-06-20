using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FntrAudit.Data;
using FntrAudit.Models;
using Microsoft.EntityFrameworkCore;

namespace FntrAudit.Services.Activites
{
    public class ActivityService : IActivityService
    {
        private readonly AppDbContext _dbContext;

        public ActivityService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Activity>> GetActivitiesAsync(CancellationToken cancellationToken = default)
        {
            var activities = await _dbContext.Activities
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return activities
                .OrderBy(a => a.intitule)
                .ToList();
        }

        public async Task<Activity?> GetActivityByIdAsync(int activityId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.id == activityId, cancellationToken);
        }

        public async Task AddActivityAsync(Activity activity, CancellationToken cancellationToken = default)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            await _dbContext.Activities.AddAsync(activity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateActivityAsync(Activity activity, CancellationToken cancellationToken = default)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            var existingActivity = await _dbContext.Activities
                .FirstOrDefaultAsync(a => a.id == activity.id, cancellationToken);

            if (existingActivity == null)
                throw new InvalidOperationException("Activité introuvable.");

            existingActivity.intitule = activity.intitule;
            existingActivity.auditId = activity.auditId;
            existingActivity.isOk = activity.isOk;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteActivityAsync(int activityId, CancellationToken cancellationToken = default)
        {
            var existingActivity = await _dbContext.Activities
                .FirstOrDefaultAsync(a => a.id == activityId, cancellationToken);

            if (existingActivity == null)
                throw new InvalidOperationException("Activité introuvable.");

            _dbContext.Activities.Remove(existingActivity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}