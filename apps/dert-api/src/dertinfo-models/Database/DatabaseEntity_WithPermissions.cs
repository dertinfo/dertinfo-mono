using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.Database
{
    /// <summary>
    /// todo - we no longer need this model and it can be completely removed. Will involve quite heavy database rollout
    ///      - with dataloss warnings as we'd be dropping the Access Token column. THis is a hangover from the legacy 
    ///      - authorisation system.
    /// </summary>
    public class DatabaseEntity_WithPermissions : DatabaseEntity
    {
        public string AccessToken { get; set; }
    }
}
