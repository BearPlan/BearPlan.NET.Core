using BearPlan.Core.Attributes;
using BearPlan.Core.Enums;

namespace BearPlan.Core.ConfigOptions;

/// <summary>
/// 运行模式
/// </summary>
[OptionsSettings]
public class RunModeOptions
{
    /// <summary>
    /// 运行模式
    /// </summary>
    public RunMode RunMode { get; set; }
}