using System;

namespace BearPlan.Core.Attributes;

/// <summary>
/// 表示不需要记录操作日志
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NotAuditAttribute : Attribute;
