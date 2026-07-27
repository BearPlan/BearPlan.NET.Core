using BearPlan.Core.Extensions;
using BearPlan.Core.Helper.Serilog;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace BearPlan.Core.Middleware;

/// <summary>
/// IP限流策略中间件
/// </summary>
public static class IpLimitMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(IpLimitMiddleware));

    public static void UseIpLimitMiddleware(this IApplicationBuilder app)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));
        try
        {
            if (App.GetOptions<MiddlewareOptions>().IpLimit)
            {
                app.UseIpRateLimiting();
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Error occured limiting ip rate.\n{e.Message}");
            throw;
        }
    }
}