using Microsoft.Extensions.DependencyInjection;
using Mst.ActivityLog.Filters;
using Mst.ActivityLog.Services;

namespace Mst.ActivityLog;

public static class DependencyInjection
{
    public static IServiceCollection AddActivityLogging(this IServiceCollection services, Action<ActivityLogSettings> configureSettings)
    {
        var activityLogSettings = new ActivityLogSettings();
        configureSettings(activityLogSettings);
        services.AddSingleton(activityLogSettings);

        services.AddScoped<IActivityLogger, ActivityLogger>();
        services.AddScoped<ActivityLogFilter>();

        return services;
    }
}
public class ActivityLogSettings
{
    public List<HttpMethodType> HttpMethods { get; set; } = new List<HttpMethodType>();

    public HttpMethodType ConvertToStandardHttpMethods(string httpMethod)
    {
        switch (httpMethod.ToLower())
        {
            case "get":
                return HttpMethodType.Get;
            case "post":
                return HttpMethodType.Post;
            case "put":
                return HttpMethodType.Put;
            case "delete":
                return HttpMethodType.Delete;
            default:
                throw new ArgumentOutOfRangeException(nameof(httpMethod), httpMethod, null);
        }
    }

}



public enum HttpMethodType
{
    Get,
    Post,
    Put,
    Delete
}

