using System.Collections.Concurrent;
using StackExchange.Redis;

namespace BearPlan.Core.Caches.Redis;

/// <summary>
/// 基于 RedisCache 持有的 ConnectionMultiplexer 实现 Pub/Sub
/// </summary>
public class RedisSubscriber : IRedisSubscriber
{
    private readonly RedisCache _redisCache;

    /// <summary>
    /// 业务回调 -> StackExchange.Redis 回调的映射：SE.Redis 按回调实例匹配退订，
    /// 必须退订时传入订阅时的同一个包装实例
    /// </summary>
    private readonly ConcurrentDictionary<(string Channel, Action<string> Handler), Action<RedisChannel, RedisValue>> _handlerMap = new();

    public RedisSubscriber(ICache cache)
    {
        // 仅 RedisCache 实现持有 ConnectionMultiplexer；分布式缓存模式下不支持订阅
        _redisCache = cache as RedisCache
            ?? throw new InvalidOperationException("RedisSubscriber 仅在 UseRedisCache=true 时可用");
    }

    /// <inheritdoc />
    public async Task SubscribeAsync(string channel, Action<string> onMessage)
    {
        var subscriber = _redisCache.GetSubscriber();
        // PatternMode.Literal 表示通道名为精确匹配，不作为通配符模式
        Action<RedisChannel, RedisValue> seHandler = (_, value) =>
        {
            if (value.HasValue)
            {
                onMessage(value.ToString());
            }
        };
        _handlerMap[(channel, onMessage)] = seHandler;
        await subscriber.SubscribeAsync(new RedisChannel(channel, RedisChannel.PatternMode.Literal), seHandler);
    }

    /// <inheritdoc />
    public async Task UnsubscribeAsync(string channel, Action<string> onMessage)
    {
        if (!_handlerMap.TryRemove((channel, onMessage), out var seHandler))
        {
            return;
        }
        var subscriber = _redisCache.GetSubscriber();
        // 带回调实例退订：只移除本订阅的回调，不影响同通道其他订阅者
        await subscriber.UnsubscribeAsync(new RedisChannel(channel, RedisChannel.PatternMode.Literal), seHandler);
    }

    /// <inheritdoc />
    public async Task PublishAsync(string channel, string message)
    {
        var subscriber = _redisCache.GetSubscriber();
        await subscriber.PublishAsync(new RedisChannel(channel, RedisChannel.PatternMode.Literal), message);
    }
}
