using System;
using System.ComponentModel.DataAnnotations;

namespace BearPlan.Core.Model;

public class ExportBase
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [Display(Name = "Sys_CreateTime")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// ID
    /// </summary>
    [Display(Name = "Sys_Id")]
    public long Id { get; set; }
}
