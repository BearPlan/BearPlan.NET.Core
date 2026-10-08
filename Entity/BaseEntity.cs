using System;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Core.Entity
{
    /// <summary>
    /// 实体基类
    /// </summary>
    [SugarIndex("index_{table}_CreateId", nameof(CreateId), OrderByType.Asc)]
    [SugarIndex("index_{table}_IsDeleted", nameof(IsDeleted), OrderByType.Asc)]
    public class BaseEntity<TKey> : RootKey<TKey>, ICreateIdEntity, IUpdateIdEntity, ISoftDeletedEntity where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// 创建者ID（恒为平台用户ID，固定 long，不随主键类型）
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true, IsNullable = true)]
        public long? CreateId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者ID（恒为平台用户ID，固定 long，不随主键类型）
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, IsNullable = true)]
        public long? UpdateId { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public DateTime? UpdateTime { get; set; }


        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
