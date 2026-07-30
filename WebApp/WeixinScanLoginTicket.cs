namespace BearPlan.Core.WebApp;

/// <summary>
/// 微信扫码登录一次性凭证（缓存于 weixin:scan:login:{ticket}，换 Token 后立即删除）
/// </summary>
public class WeixinScanLoginTicket
{
    /// <summary>
    /// 已绑定的系统用户 Id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 登录平台（客户端类型字符串形式，对应 VersionEnum 的名称，如 Pc、UDhold）
    /// </summary>
    public string ApiVersion { get; set; } = string.Empty;
}
