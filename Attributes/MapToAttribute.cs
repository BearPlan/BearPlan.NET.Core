using System;

namespace BearPlan.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MapToAttribute : Attribute
{
    public MapToAttribute(Type targetType)
    {
        TargetType = targetType;
    }

    public Type TargetType { get; }
}
