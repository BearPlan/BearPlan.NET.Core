namespace BearPlan.Core.WebApp;

/// <summary>
/// 当前用户
/// </summary>
public interface IHttpUser
{
    /// <summary>
    /// 当前登录用户ID
    /// </summary>
    long Id { get; }

    /// <summary>
    /// 当前登录用户名称
    /// </summary>
    string Account { get; }

    /// <summary>
    /// 部门ID
    /// </summary>
    long DeptId { get; }

    /// <summary>
    /// 租户ID
    /// </summary>
    int TenantId { get; }

    /// <summary>
    /// 请求携带的Token
    /// </summary>
    /// <returns></returns>
    string JwtToken { get; }
    /// <summary>
    /// 是否已认证
    /// </summary>
    /// <returns></returns>
    bool IsAuthenticated { get; }

    /// <summary>
    /// 客户端类型（原 VersionEnum 的字符串形式，如 Pc、UDhold、Plugin 等）
    /// </summary>
    string ApiVersion { get; }

    /// <summary>
    /// 设备ID，标识同一用户的同一台设备，用于多端在线区分与定向推送
    /// </summary>
    string DeviceId { get; }
}
