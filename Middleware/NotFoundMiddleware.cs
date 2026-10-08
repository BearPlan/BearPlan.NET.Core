using BearPlan.Core.Extensions;
using BearPlan.Core.Model;
using Microsoft.AspNetCore.Http;

namespace BearPlan.Core.Middleware;

public class NotFoundMiddleware
{
    private readonly RequestDelegate _next;

    public NotFoundMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
        {
            // 设置ContentType
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(ExcutedResult<object>.FailedResult(data: new
            {
                Status = StatusCodes.Status404NotFound,
                ActionError = new ActionError(),
                Message = "The request failed. The access interface does not exist.",
                Path = context.Request.Path.Value?.ToLower()
            }, code: StatusCodes.Status404NotFound).ToJson());
        }
    }
}
