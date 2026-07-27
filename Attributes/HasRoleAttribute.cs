using System;

namespace BearPlan.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HasRoleAttribute : Attribute
{
    public HasRoleAttribute(string[] authCodes)
    {
        AuthCodes = authCodes;
    }

    /// <summary>
    /// 权限代码
    /// </summary>
    public string[] AuthCodes { get; }
}