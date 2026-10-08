namespace BearPlan.Core.Model;

/// <summary>
/// 创建人ID接口（创建人恒为平台用户，固定 long，与实体自身主键类型无关；用户数据权限过滤器挂此接口）
/// </summary>
public interface ICreateIdEntity
{
    /// <summary>
    /// 创建人ID
    /// </summary>
    long? CreateId { get; set; }
}
