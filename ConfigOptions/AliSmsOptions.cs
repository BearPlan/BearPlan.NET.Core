using BearPlan.Core.Attributes;

namespace BearPlan.Core.ConfigOptions;

/// <summary>
/// 阿里云短信配置（仅短信发送相关）
/// </summary>
[OptionsSettings]
public class AliSmsOptions
{
    /// <summary>
    /// 是否启用真实短信发送（关闭时使用固定验证码 123457，便于联调）
    /// </summary>
    public bool IsSmsEnable { get; set; }

    /// <summary>
    /// 阿里云 AccessKeyId
    /// </summary>
    public string AccessKeyId { get; set; } = string.Empty;

    /// <summary>
    /// 阿里云 AccessKeySecret
    /// </summary>
    public string AccessKeySecret { get; set; } = string.Empty;

    /// <summary>
    /// 短信签名
    /// </summary>
    public string SignName { get; set; } = string.Empty;

    /// <summary>
    /// 短信模板编号
    /// </summary>
    public string TemplateCode { get; set; } = string.Empty;

    /// <summary>
    /// 阿里云 dysmsapi 接入域名
    /// </summary>
    public string Endpoint { get; set; } = "dysmsapi.aliyuncs.com";

    /// <summary>
    /// 验证码缓存时长（分钟）
    /// </summary>
    public int CodeTtlMinutes { get; set; } = 2;

    /// <summary>
    /// 同一手机号重复发送的冷却时长（秒）
    /// </summary>
    public int SendLockSeconds { get; set; } = 60;
}
