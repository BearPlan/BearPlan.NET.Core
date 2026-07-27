using System;
using BearPlan.Core.Attributes;
using SqlSugar;

namespace BearPlan.Core.Model;

/// <summary>
/// 日期范围
/// </summary>
public class DateRange
{
    /// <summary>
    /// 开始[0]--结束[1]
    /// </summary>
    // [QueryCondition(ConditionType = ConditionalType.Range)]
    // public List<DateTime> CreateTime { get; set; }


    [QueryCondition(ConditionType = ConditionalType.GreaterThanOrEqual, FieldName = "CreateTime")]
    public DateTime? StartTime { get; set; }

    [QueryCondition(ConditionType = ConditionalType.LessThanOrEqual, FieldName = "CreateTime")]
    public DateTime? EndTime { get; set; }
}
