using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Outlay.Infrastructure.Extensions;

public static class DistributedCacheExtension
{
    public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recodeId, T data,
        TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpirationTime = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
            SlidingExpiration = slidingExpirationTime
        };
        var jsonData = JsonConvert.SerializeObject(data);
        await cache.SetStringAsync(recodeId, jsonData, options);
    }

    public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
    {
        var jsonData = await cache.GetStringAsync(recordId);
        return jsonData is null 
            ? default
            : JsonConvert.DeserializeObject<T>(jsonData);
    }
}