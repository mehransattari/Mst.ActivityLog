﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Mst.ActivityLog.Services;

namespace Mst.ActivityLog.Filters;

public class ActivityLogFilter : IActionFilter
{
    private readonly IActivityLogger _activityLogger;

    public ActivityLogFilter(IActivityLogger activityLogger)
    {
        _activityLogger = activityLogger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var httpMethod = context.HttpContext.Request.Method;
        if (httpMethod == HttpMethods.Post || httpMethod == HttpMethods.Delete)
        {
            var activityLog = new ActivityLog
            {
                UserId = context.HttpContext.User.Identity.Name ?? "No User"    ,
                ActionType = httpMethod,
                Description = httpMethod == HttpMethods.Post ? "Adding new item" : "Deleting an item",
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
        var httpMethod = context.HttpContext.Request.Method;
        if (httpMethod == HttpMethods.Post || httpMethod == HttpMethods.Delete)
        {
            var activityLog = new ActivityLog
            {
                UserId = context.HttpContext.User.Identity.Name ?? "No User",
                ActionType = httpMethod,
                Date = DateTime.UtcNow,
                IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = context.HttpContext.Request.Headers["User-Agent"],
                ControllerName = context.ActionDescriptor.RouteValues["controller"],
                ActionName = context.ActionDescriptor.RouteValues["action"],
                Description = context.Exception != null
                    ? $"Operation failed due to: {context.Exception.Message}"
                    : "Operation succeeded"
            };

            _activityLogger.LogAsync(activityLog).GetAwaiter().GetResult();
        }
    }
}
