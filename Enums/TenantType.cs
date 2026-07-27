using System.ComponentModel.DataAnnotations;

namespace BearPlan.Core.Enums;

/// <summary>
/// 租户类型
/// </summary>
public enum TenantType
{
    /// <summary>
    /// Id隔离
    /// </summary>
    [Display(Name = "Enum_Tenant_Id")]
    Id = 1,

    /// <summary>
    /// 库隔离
    /// </summary>
    [Display(Name = "Enum_Tenant_Db")]
    Db = 2
}
