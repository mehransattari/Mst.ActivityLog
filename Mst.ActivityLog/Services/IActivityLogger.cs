namespace Mst.ActivityLog.Services;

public interface IActivityLogger
{
    Task LogAsync(ActivityLog activityLog);
}
