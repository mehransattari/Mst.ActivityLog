using Microsoft.EntityFrameworkCore;

namespace Mst.ActivityLog;

public interface IActivityLogDbContext
{
    DbSet<ActivityLog> ActivityLogs { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

