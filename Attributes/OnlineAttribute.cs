using System;

namespace BearPlan.Core.Attributes;

/// <summary>
/// 自定义鉴权特性，在线则可通行
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnlineAttribute : Attribute
{
}
