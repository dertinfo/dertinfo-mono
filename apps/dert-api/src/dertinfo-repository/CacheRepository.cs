using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface ICacheRepository : IRepository<DatabaseCacheItem, int>
    {
        Task<bool> HasCache(string cacheKey);
        Task<bool> AddCacheItem(DatabaseCacheItem item);
        Task<bool> ClearCacheItem(string cacheKey);
        Task<DatabaseCacheItem> GetCacheItem(string cacheKey);
    }

    public class CacheRepository : BaseRepository<DatabaseCacheItem, int, DertInfoContext>, ICacheRepository
    {
        public CacheRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<bool> HasCache(string cacheKey)
        {
            var task = Task.Run(() =>
            {
                return this._context.DatabaseCache.Any(c => c.CacheKey == cacheKey);
            });

            return await task;
        }

        public async Task<bool> AddCacheItem(DatabaseCacheItem item)
        {
            var conflict = await base.SingleOrDefault(c => c.CacheKey == item.CacheKey);

            if (conflict != null)
            {
                await base.Delete(conflict);
            }

            await base.Add(item);

            return true;
        }

        public async Task<bool> ClearCacheItem(string cacheKey)
        {
            var found = _context.DatabaseCache.Where(dc => dc.CacheKey == cacheKey).FirstOrDefault();

            if (found != null)
            {
                await base.Delete(found);
            }

            return true;
        }

        public async Task<DatabaseCacheItem> GetCacheItem(string cacheKey)
        {
            return await base.SingleOrDefault(c => c.CacheKey == cacheKey);
        }
    }
}
