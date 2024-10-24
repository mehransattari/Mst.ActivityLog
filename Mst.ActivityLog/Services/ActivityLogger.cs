namespace Mst.ActivityLog.Services;


public class ActivityLogger : IActivityLogger
{
    private readonly IActivityLogDbContext _context;

    public ActivityLogger(IActivityLogDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(ActivityLog activityLog)
    {
        _context.ActivityLogs.Add(activityLog);
        await _context.SaveChangesAsync();
    }
}
