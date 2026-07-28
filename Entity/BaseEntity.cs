using System;
using BearPlan.Core.Model;
using SqlSugar;

namespace BearPlan.Core.Entity
{
    /// <summary>
    /// 实体基类
    /// </summary>
    [SugarIndex("index_{table}_CreateBy", nameof(CreateBy), OrderByType.Asc)]
    [SugarIndex("index_{table}_IsDeleted", nameof(IsDeleted), OrderByType.Asc)]
    public class BaseEntity<TKey> : RootKey<TKey>, ICreateByEntity, ISoftDeletedEntity where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 创建者名称
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true, IsNullable = true)]
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者名称
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, IsNullable = true)]
        public string UpdateBy { get; set; } = string.Empty;

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
