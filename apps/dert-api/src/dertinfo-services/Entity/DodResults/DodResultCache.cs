using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using DertInfo.Repository;
using DertInfo.Services.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodResults
{
    public interface IDodResultCache: ICacheService<DodTeamCollatedResultPairDO>
    {
        Task<DodTeamCollatedResultPairDO> GetCache();
        Task CreateCache(DodTeamCollatedResultPairDO data);
        Task<bool> ClearCache();
    }

    public class DodResultCache : CacheService<DodTeamCollatedResultPairDO>,  IDodResultCache
    {
        private const string CACHEKEY = "DODRESULTS";

        public DodResultCache(ICacheRepository cacheRepository, IDertInfoConfiguration configuration) : base(cacheRepository, configuration)
        {

        }

        public async Task<DodTeamCollatedResultPairDO> GetCache()
        {
            if (await base.HasCache(CACHEKEY))
            {
                return await base.GetCacheItemSingle(CACHEKEY);
            }

            return null;
        }

        public async Task CreateCache(DodTeamCollatedResultPairDO data)
        {
            await base.AddCacheItemSingle(CACHEKEY, data);
        }

        public async Task<bool> ClearCache()
        {
            await base.ClearCacheItem(CACHEKEY);
            return true;
        }


    }
}
