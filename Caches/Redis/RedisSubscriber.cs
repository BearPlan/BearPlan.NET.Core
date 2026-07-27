using StackExchange.Redis;

namespace BearPlan.Core.Caches.Redis;

/// <summary>
/// 基于 RedisCache 持有的 ConnectionMultiplexer 实现 Pub/Sub
/// </summary>
public class RedisSubscriber : IRedisSubscriber
{
    private readonly RedisCache _redisCache;

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
        await subscriber.SubscribeAsync(new RedisChannel(channel, RedisChannel.PatternMode.Literal), (_, value) =>
        {
            if (value.HasValue)
            {
                onMessage(value.ToString());
            }
        });
    }

    /// <inheritdoc />
    public async Task PublishAsync(string channel, string message)
    {
        var subscriber = _redisCache.GetSubscriber();
        await subscriber.PublishAsync(new RedisChannel(channel, RedisChannel.PatternMode.Literal), message);
    }
}
