using System;

namespace BearPlan.Core.Attributes;

/// <summary>
/// 租户标识(库隔离)
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MultiDbTenantAttribute : Attribute
{
}
