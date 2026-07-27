using Microsoft.AspNetCore.Http;

namespace BearPlan.Core.Middleware;

/// <summary>
/// CORS 预检放行中间件
/// <para>[NotCors]/[EnableCors] 基于 endpoint 元数据，但 OPTIONS 预检不会命中标记在
/// GET/POST 等 action 上的策略（方法不匹配 → 回退默认白名单策略，导致来源不在白名单时无 CORS 头）。
/// 此中间件对带 Origin 的预检统一放行任意来源；真实请求仍由 [NotCors]→AllowAll 或全局白名单策略
/// 各自校验来源，故放行预检不会削弱受保护接口的来源限制（不在白名单的来源，真实请求照样被挡掉）。</para>
/// </summary>
public class CorsMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托
    /// </summary>
    /// <param name="next"></param>
    public CorsMiddleware(RequestDelegate next) => _next = next;

    /// <summary>
    /// 自定义中间件要执行的逻辑
    /// </summary>
    /// <param name="context"></param>
    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        // 仅处理 CORS 预检：OPTIONS + Origin + Access-Control-Request-Method
        if (HttpMethods.IsOptions(request.Method)
            && request.Headers.Origin.Count > 0
            && request.Headers.AccessControlRequestMethod.Count > 0)
        {
            var headers = context.Response.Headers;
            // 回显来源（而非通配 *），便于将来与 AllowCredentials 共存
            headers.AccessControlAllowOrigin = request.Headers.Origin.ToString();
            headers.AccessControlAllowMethods = "GET, POST, PUT, DELETE, OPTIONS";
            headers.AccessControlAllowHeaders = string.IsNullOrEmpty(request.Headers.AccessControlRequestHeaders)
                ? "*"
                : request.Headers.AccessControlRequestHeaders.ToString();
            headers.AccessControlMaxAge = "86400";
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return; // 预检直接返回，不进入后续管道
        }

        await _next(context); // 非预检请求交给后续中间件处理
    }
}
