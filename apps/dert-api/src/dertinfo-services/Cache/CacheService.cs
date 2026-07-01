using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Cache
{
    public interface ICacheService<T>
    {
        Task<bool> HasCache(string cacheKey);

        Task AddCacheItemSingle(string cacheKey, T myObject);

        Task AddCacheItemEnumerable(string cacheKey, IEnumerable<T> myObject);

        Task ClearCacheItem(string cacheKey);

        Task<T> GetCacheItemSingle(string cacheKey);

        Task<IEnumerable<T>> GetCacheItemEnumerable(string cacheKey);
    }

    public class CacheService<T> : ICacheService<T>
    {
        private ICacheRepository _cacheRepository;
        private IDertInfoConfiguration _configuration;

        public CacheService(
            
            ICacheRepository cacheRepository,
            IDertInfoConfiguration configuration
            )
        {
            _cacheRepository = cacheRepository;
            _configuration = configuration;
        }

        public async Task<bool> HasCache(string cacheKey)
        {
            if (_configuration.DatabaseCache_Disabled) { return false; }

            return await this._cacheRepository.HasCache(cacheKey);
        }

        public async Task AddCacheItemSingle(string cacheKey, T myObject)
        {
            if (_configuration.DatabaseCache_Disabled) { return; }

            var cacheContainerJson = JsonConvert.SerializeObject(myObject, Formatting.None);

            var cacheItem = new DatabaseCacheItem()
            {
                CacheKey = cacheKey,
                JsonObject = cacheContainerJson
            };

            await this._cacheRepository.AddCacheItem(cacheItem);
        }

        public async Task AddCacheItemEnumerable(string cacheKey, IEnumerable<T> myObject)
        {
            if (_configuration.DatabaseCache_Disabled) { return; }

            var json = JsonConvert.SerializeObject(myObject, Formatting.None);

            var cacheItem = new DatabaseCacheItem()
            {
                CacheKey = cacheKey,
                JsonObject = json
            };

            await this._cacheRepository.AddCacheItem(cacheItem);
        }

        public async Task ClearCacheItem(string cacheKey)
        {
            await this._cacheRepository.ClearCacheItem(cacheKey);
        }

        public async Task<T> GetCacheItemSingle(string cacheKey)
        {
            if (_configuration.DatabaseCache_Disabled) { throw new Exception($"Cache is disabled"); }

            var cacheItem = await this._cacheRepository.GetCacheItem(cacheKey);

            if (cacheItem != null)
            {
                T myObject = JsonConvert.DeserializeObject<T>(cacheItem.JsonObject, GetSerialiserSettings());

                return myObject;
            }

            throw new Exception($"Failure attempting to retrieve cache for {cacheKey} no cache found");
        }

        public async Task<IEnumerable<T>> GetCacheItemEnumerable(string cacheKey)
        {
            if (_configuration.DatabaseCache_Disabled) { throw new Exception($"Cache is disabled"); }

            var cacheItem = await this._cacheRepository.GetCacheItem(cacheKey);

            if (cacheItem != null)
            {
                IEnumerable<T> myObject = JsonConvert.DeserializeObject<IEnumerable<T>>(cacheItem.JsonObject, GetSerialiserSettings());

                return myObject;
            }

            throw new Exception($"Failure attempting to retrieve cache for {cacheKey} no cache found");
        }

        protected static JsonSerializerSettings GetSerialiserSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.None
            };
        }
    }
}
