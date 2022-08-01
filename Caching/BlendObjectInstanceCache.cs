using EPiServer.Framework.Cache;
using System;

namespace Blend.Optimizely.Caching
{
    public class BlendObjectInstanceCache : AbstractCacheService
    {
        private readonly ISynchronizedObjectInstanceCache objectCacheService;

        public BlendObjectInstanceCache(ISynchronizedObjectInstanceCache objectCacheService)
        {
            this.objectCacheService = objectCacheService;
        }


        public override TValue? Get<TValue>(string cacheKey) where TValue : class
        {
            var cacheObject = objectCacheService.Get(cacheKey);
            if (cacheObject is null)
                return default;

            if (cacheObject is TValue castValue)
                return castValue;

            return default;
        }

        public override TValue Get<TValue>(string cacheKey, CacheEvictionPolicy cacheEvictionPolicy, Func<TValue> getItemCallback)
        {
            TValue? item = Get<TValue>(cacheKey);
            if (item is not null)
                return item;

            var updatedItem = getItemCallback();
            objectCacheService.Insert(cacheKey, updatedItem, cacheEvictionPolicy);
            return updatedItem;
        }

        public override TValue Get<TValue, TId>(string cacheKeyFormat, TId id, int durationInMinutes, Func<TId, TValue> getItemCallback)
        {
            string cacheKey = string.Format(cacheKeyFormat, id);
            TValue? item = Get<TValue>(cacheKey);
            if (item is not null)
                return item;

            var updatedItem = getItemCallback(id);
            objectCacheService.Insert(cacheKey, updatedItem, new CacheEvictionPolicy(TimeSpan.FromMinutes(durationInMinutes), CacheTimeoutType.Absolute));
            return updatedItem;
        }

        public override void Remove(string cacheKey) => objectCacheService.Remove(cacheKey);
    }
}