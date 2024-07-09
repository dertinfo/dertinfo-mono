using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Database
{
    public class DatabaseCacheItem: DatabaseEntity
    {
        public string CacheKey { get; set; }

        public string JsonObject { get; set; }
    }
}
