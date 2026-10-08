namespace BearPlan.Core.Model;

/// <summary>
/// 更新人ID接口（更新人恒为平台用户，固定 long，与实体自身主键类型无关）
/// </summary>
public interface IUpdateIdEntity
{
    /// <summary>
    /// 更新人ID
    /// </summary>
    long? UpdateId { get; set; }
}
