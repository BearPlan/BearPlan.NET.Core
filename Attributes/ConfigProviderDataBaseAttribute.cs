using System;

namespace BearPlan.Core.Attributes;

/// <summary>
/// 配置库
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class ConfigProviderDataBaseAttribute : Attribute
{
}
