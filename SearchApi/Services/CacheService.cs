using Microsoft.Extensions.Caching.Memory;

namespace SearchApi.Services
{
    public class CacheService: ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;   
        }
        /// <summary>
        /// Get any object = T from cache by key
        /// </summary>
        /// <param name="key">The key of object to get from cache</param>
        /// <returns>The object = T  that matches the key</returns>
        /// <remarks>
        /// This method is used for retrieve objects saved in cache at an earlier stage.
        /// It is suitable for any type of object: T 
        /// If no suitable object is found for the key passed to the function- null will be returned
        /// </remarks>
        public T? GetItem<T>(string key)
        {
            try
            {
                _cache.TryGetValue(key, out object? obj);
                if (obj is T)
                    return (T)obj;
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred in CacheService.cs at GetItem<T> function: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Save any object = T in cache with key
        /// </summary>
        /// <param name="key">The key to save in cache.</param>
        /// <param name="item">The item to save in cache.</param>
        /// <returns>Nothing</returns>
        /// <remarks>
        /// This method is used for save object in cache. 
        /// It is suitable for any type of object: T 
        /// The object will be saved for 30 minutes
        /// </remarks>
        public void SaveItem<T>(string key, T item)
        {
            try
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                _cache.Set(key, item, cacheOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred in CacheService.cs at SaveItem<T> function: {ex.Message}");
                throw;
            }
        }
    }
}
