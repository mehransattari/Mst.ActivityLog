using Microsoft.AspNetCore.Mvc.Filters;
using Mst.ActivityLog.Services;

namespace Mst.ActivityLog.Filters;

public class ActivityLogFilter : IActionFilter
{
    private readonly IActivityLogger _activityLogger;
    private readonly ActivityLogSettings _activityLogSettings;
    public ActivityLogFilter(IActivityLogger activityLogger, ActivityLogSettings activityLogSettings)
    {
        _activityLogger = activityLogger;
        _activityLogSettings = activityLogSettings; 
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
            return;

        var httpMethod = _activityLogSettings.ConvertToStandardHttpMethods(context.HttpContext.Request.Method);

        var mobile = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("mobilephone"))?.Value ?? "No User";
        var userId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))?.Value ?? "No User";

        string description = string.Empty;

        if (httpMethod == HttpMethodType.Post)
            description = "Adding new item";

        if (httpMethod == HttpMethodType.Put)
            description = "Puting new item";

        if (httpMethod == HttpMethodType.Delete)
            description = "Deleting new item";

        if (httpMethod == HttpMethodType.Get)
            description = "Geting new item";

        if (_activityLogSettings.HttpMethods.Contains(httpMethod))
        {          
            var activityLog = new ActivityLog
            {
                UserId = userId,
                ActionType = httpMethod.ToString(),
                Description = description + " | Mobile: " + mobile ,
                Date = DateTime.UtcNow,
                IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = context.HttpContext.Request.Headers["User-Agent"],
                ControllerName = context.ActionDescriptor.RouteValues["controller"],
                ActionName = context.ActionDescriptor.RouteValues["action"]
            };

            _activityLogger.LogAsync(activityLog).GetAwaiter().GetResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
            return;

        var httpMethod = _activityLogSettings.ConvertToStandardHttpMethods(context.HttpContext.Request.Method);
        var mobile = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("mobilephone"))?.Value ?? "No User";
        var userId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))?.Value ?? "No User";

        if (_activityLogSettings.HttpMethods.Contains(httpMethod))
        {

            var activityLog = new ActivityLog
            {
                UserId = userId,
                ActionType = httpMethod.ToString(),
                Date = DateTime.UtcNow,
                IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = context.HttpContext.Request.Headers["User-Agent"],
                ControllerName = context.ActionDescriptor.RouteValues["controller"],
                ActionName = context.ActionDescriptor.RouteValues["action"],
                Description = context.Exception != null
                              ? $"Operation failed due to: {context.Exception.Message}"
                              : "Operation succeeded | Mobile: " + mobile
            };

            _activityLogger.LogAsync(activityLog).GetAwaiter().GetResult();
        }
    }
}
