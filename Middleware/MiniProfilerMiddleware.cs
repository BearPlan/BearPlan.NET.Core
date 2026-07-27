using BearPlan.Core.Extensions;
using BearPlan.Core.Helper.Serilog;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace BearPlan.Core.Middleware;

/// <summary>
/// 性能监控中间件
/// </summary>
public static class MiniProfilerMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(MiniProfilerMiddleware));

    public static void UseMiniProfilerMiddleware(this IApplicationBuilder app)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));
        try
        {
            if (App.GetOptions<MiddlewareOptions>().MiniProfiler)
            {
                // 性能分析
                app.UseMiniProfiler();
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Performance monitoring startup error:\n{e.Message}");
            throw;
        }
    }
}