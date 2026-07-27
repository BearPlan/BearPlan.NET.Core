using System;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace BearPlan.Core.Attributes;

/// <summary>
/// 跳过CORS限制(允许任意来源访问)
/// <para>标记到 Controller 或 Action 后，该接口将使用 <see cref="AllowAllPolicy"/> 策略，覆盖全局CORS策略</para>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class NotCorsAttribute : Attribute, IEnableCorsAttribute
{
    /// <summary>
    /// 跳过CORS限制(允许任意来源)的策略名，需在 <c>AddCorsSetup</c> 中注册同名策略
    /// </summary>
    public const string AllowAllPolicy = "AllowAll";

    /// <summary>
    /// 使用的CORS策略名
    /// </summary>
    public string PolicyName { get; set; } = AllowAllPolicy;
}
