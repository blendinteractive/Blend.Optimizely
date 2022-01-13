using EPiServer.Framework.Cache;
using System;

namespace Blend.Optimizely.Caching
{
    public interface ICacheService
    {
        TValue Get<TValue>(string cacheKey) where TValue : class;

        TValue Get<TValue>(string cacheKey, Func<TValue> getItemCallback) where TValue : class;

        TValue Get<TValue>(string cacheKey, CacheEvictionPolicy cacheEvictionPolicy, Func<TValue> getItemCallback) where TValue : class;

        TValue Get<TValue>(string cacheKey, int durationInMinutes, Func<TValue> getItemCallback) where TValue : class;

        TValue Get<TValue, TId>(string cacheKeyFormat, TId id, Func<TId, TValue> getItemCallback) where TValue : class;

        TValue Get<TValue, TId>(string cacheKeyFormat, TId id, int durationInMinutes, Func<TId, TValue> getItemCallback) where TValue : class;

        void Remove(string cacheKey);

        void Remove<TId>(string cacheKey, TId id);
    }
}
