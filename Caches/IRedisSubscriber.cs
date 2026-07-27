namespace BearPlan.Core.Caches;

/// <summary>
/// Redis 发布/订阅能力，用于跨实例事件通知（如扫码登录 SSE 推送）
/// </summary>
public interface IRedisSubscriber
{
    /// <summary>
    /// 订阅指定通道，收到消息时触发回调
    /// </summary>
    /// <param name="channel">通道名称</param>
    /// <param name="onMessage">消息回调（参数为通道收到的消息内容）</param>
    Task SubscribeAsync(string channel, Action<string> onMessage);

    /// <summary>
    /// 向指定通道发布消息
    /// </summary>
    /// <param name="channel">通道名称</param>
    /// <param name="message">消息内容</param>
    Task PublishAsync(string channel, string message);
}
