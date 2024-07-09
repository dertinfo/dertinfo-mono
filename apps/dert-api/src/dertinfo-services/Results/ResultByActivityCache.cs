using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.System.Results;
using DertInfo.Repository;
using DertInfo.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Results
{
    public interface IResultByActivityCache : ICacheService<TeamCollatedResult>
    {
        Task<IEnumerable<TeamCollatedResult>> GetCache(int activityId, int[] scoreCategories);
        Task CreateCache(int activityId, int[] scoreCategories, IEnumerable<TeamCollatedResult> data);
        Task ClearCache(int activityId, int[] scoreCategories);
    }

    public class ResultByActivityCache : CacheService<TeamCollatedResult>, IResultByActivityCache
    {
        private const string CACHEKEYPREFIX = "RESULTS";

        public ResultByActivityCache(ICacheRepository cacheRepository, IDertInfoConfiguration configuration) : base(cacheRepository, configuration)
        {

        }

        public async Task<IEnumerable<TeamCollatedResult>> GetCache(int activityId, int[] scoreCategories)
        {
            var cacheKey = BuildCacheKey(activityId, scoreCategories);
            if (await base.HasCache(cacheKey))
            {
                return await base.GetCacheItemEnumerable(cacheKey);
            }

            return null;
        }

        public async Task CreateCache(int activityId, int[] scoreCategories, IEnumerable<TeamCollatedResult> data)
        {
            var cacheKey = BuildCacheKey(activityId, scoreCategories);
            await base.AddCacheItemEnumerable(cacheKey, data);
        }

        public async Task ClearCache(int activityId, int[] scoreCategories)
        {
            var cacheKey = BuildCacheKey(activityId, scoreCategories);
            await base.ClearCacheItem(cacheKey);
        }

        /// <summary>
        /// Builds the key to be used for the results cache.
        /// </summary>
        /// <param name="activiyId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private string BuildCacheKey(int activiyId, int[] scoreCategories)
        {
            var ordered = scoreCategories.OrderBy(i => i);
            var concat = String.Concat(ordered);

            return $"{CACHEKEYPREFIX}_A{activiyId.ToString()}_C{concat}";
        }


    }
}
