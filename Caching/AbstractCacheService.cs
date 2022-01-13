using EPiServer.Framework.Cache;

namespace Blend.Optimizely.Caching
{
    public abstract class AbstractCacheService : ICacheService
    {
        private static readonly TimeSpan _DefaultCacheDuration;
        private static readonly CacheEvictionPolicy _DefaultCacheEvictionPolicy;

        protected virtual TimeSpan DefaultCacheDuration => _DefaultCacheDuration;

        protected virtual CacheEvictionPolicy DefaultCacheEvictionPolicy => _DefaultCacheEvictionPolicy;

        static AbstractCacheService()
        {
            _DefaultCacheDuration = TimeSpan.FromHours(24);
            _DefaultCacheEvictionPolicy = new CacheEvictionPolicy(_DefaultCacheDuration, CacheTimeoutType.Absolute);
        }

        public abstract TValue Get<TValue>(string cacheKey) where TValue : class;

        public abstract TValue Get<TValue>(string cacheKey, CacheEvictionPolicy cacheEvictionPolicy, Func<TValue> getItemCallback) where TValue : class;

        public TValue Get<TValue>(string cacheKey, Func<TValue> getItemCallback) where TValue : class
            => Get(cacheKey, DefaultCacheEvictionPolicy, getItemCallback);

        public TValue Get<TValue>(string cacheKey, int durationInMinutes, Func<TValue> getItemCallback) where TValue : class
            => Get(cacheKey, new CacheEvictionPolicy(TimeSpan.FromMinutes(durationInMinutes), CacheTimeoutType.Absolute), getItemCallback);

        public abstract TValue Get<TValue, TId>(string cacheKeyFormat, TId id, int durationInMinutes, Func<TId, TValue> getItemCallback) where TValue : class;

        public TValue Get<TValue, TId>(string cacheKeyFormat, TId id, Func<TId, TValue> getItemCallback) where TValue : class
            => Get(cacheKeyFormat, id, (int)DefaultCacheDuration.TotalMinutes, getItemCallback);

        protected virtual string FormatKey<TId>(string cacheKeyFormat, TId id) => string.Format(cacheKeyFormat, id);

        public abstract void Remove(string cacheKey);

        public void Remove<TId>(string cacheKey, TId id) => Remove(FormatKey(cacheKey, id));
    }
}