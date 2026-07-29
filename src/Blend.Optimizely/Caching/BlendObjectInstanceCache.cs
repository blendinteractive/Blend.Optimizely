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
            => objectCacheService.Get<TValue>(cacheKey, ReadStrategy.Wait);

        public override TValue Get<TValue>(string cacheKey, CacheEvictionPolicy cacheEvictionPolicy, Func<TValue> getItemCallback)
            => objectCacheService.ReadThrough(cacheKey, getItemCallback, _ => cacheEvictionPolicy, ReadStrategy.Wait);

        public override TValue Get<TValue, TId>(string cacheKeyFormat, TId id, int durationInMinutes, Func<TId, TValue> getItemCallback)
        {
            string cacheKey = FormatKey(cacheKeyFormat, id);
            return objectCacheService.ReadThrough(
                cacheKey,
                () => getItemCallback(id),
                _ => new CacheEvictionPolicy(TimeSpan.FromMinutes(durationInMinutes), CacheTimeoutType.Absolute),
                ReadStrategy.Wait);
        }

        public override void Remove(string cacheKey) => objectCacheService.Remove(cacheKey);
    }
}