using Asp.Versioning.ApiExplorer;
using BearPlan.Core;
using BearPlan.Core.ConfigOptions;
using BearPlan.Core.Extensions;
using BearPlan.Core.Helper.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Serilog;

namespace BearPlan.Core.Middleware;


/// <summary>
/// Swagger UI 中间件
/// </summary>
public static class SwaggerUiMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(SwaggerUiMiddleware));

    /// <param name="versionLabel">可选的版本标签映射：传入主版本号返回 Swagger 下拉显示文本；不传则回退显示 GroupName（如 "1.0"）</param>
    public static void UseSwaggerUiMiddleware(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, Func<Stream> streamHtml, Func<int, string>? versionLabel = null)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));
        var swaggerOptions = App.GetOptions<SwaggerOptions>();
        if (swaggerOptions.Enabled)
        {

            //app.UseSwagger();
            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((doc, item) =>
                {
                    //根据代理服务器提供的协议、地址和路由，生成api文档服务地址
                    doc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer
                            { Url = $"{item.Scheme}://{item.Host.Value}" }
                    };
                });
            });
            app.UseSwaggerUI(options =>
            {

                // 添加文档信息
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    //这个属性是往SwaggerUI页面head标签中添加我们自己的代码，比如引入一些样式文件，或者执行自己的一些脚本代码
                    //options.HeadContent += $"<script type='text/javascript'>alert('欢迎来到SwaggerUI页面')</script>";

                    //展示默认头部显示的下拉版本信息
                    //默认行为：options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    //自由指定头部显示的下拉版本内容：调用方可传入版本号→中文标签的映射（如业务侧的 VersionEnum 映射）
                    var majorVersion = description.ApiVersion.MajorVersion ?? 0;
                    var label = versionLabel?.Invoke(majorVersion) ?? description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", label);
                    //options.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("MiniProfilerSample.index.html");
                    //如果是为空 访问路径就为 根域名/index.html,注意localhost:3001/swagger是访问不到的
                    //options.RoutePrefix = string.Empty;
                    // 如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "swagger"; 则访问路径为 根域名/swagger/index.html

                    //{
                    //    url = "/swagger/logo.png" // 添加 logo
                    //};
                    // 启用深色模式等增强功能
                    options.ConfigObject.DeepLinking = true;
                    options.DisplayRequestDuration();
                }
                ;
                options.RoutePrefix = swaggerOptions.Route;

                var stream = streamHtml?.Invoke();
                if (stream == null)
                {
                    const string msg = "index.html属性错误";
                    Logger.Error(msg);
                    throw new System.Exception(msg);
                }
                options.IndexStream = streamHtml;
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });
        }
    }
}
