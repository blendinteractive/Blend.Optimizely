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

        public override TValue Get<TValue>(string cacheKey) => objectCacheService.Get(cacheKey) as TValue;

        public override TValue Get<TValue>(string cacheKey, CacheEvictionPolicy cacheEvictionPolicy, Func<TValue> getItemCallback)
        {
            TValue item = Get<TValue>(cacheKey);
            if (item == null)
            {
                item = getItemCallback();
                if (item != null)
                    objectCacheService.Insert(cacheKey, item, cacheEvictionPolicy);
            }
            return item;
        }

        public override TValue Get<TValue, TId>(string cacheKeyFormat, TId id, int durationInMinutes, Func<TId, TValue> getItemCallback)
        {
            string cacheKey = string.Format(cacheKeyFormat, id);
            TValue item = Get<TValue>(cacheKey);
            if (item == null)
            {
                item = getItemCallback(id);
                if (item != null)
                    objectCacheService.Insert(cacheKey, item, new CacheEvictionPolicy(TimeSpan.FromMinutes(durationInMinutes), CacheTimeoutType.Absolute));
            }
            return item;
        }

        public override void Remove(string cacheKey) => objectCacheService.Remove(cacheKey);
    }
}