using Microsoft.Extensions.DependencyInjection;
using Mst.ActivityLog.Filters;
using Mst.ActivityLog.Services;

namespace Mst.ActivityLog;

public static class DependencyInjection
{
    public static IServiceCollection AddActivityLogging(this IServiceCollection services, Action<ActivityLogSettings> configureSettings)
    {
        var settings = new ActivityLogSettings();
        configureSettings(settings);
        services.AddSingleton(settings);

        services.AddScoped<IActivityLogger, ActivityLogger>();
        services.AddScoped<ActivityLogFilter>();

        return services;
    }
}
public class ActivityLogSettings
{
    public List<HttpMethodType> HttpMethods { get; set; } = new List<HttpMethodType>();
}


public enum HttpMethodType
{
    Get,
    Post,
    Put,
    Delete
}

