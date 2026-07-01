using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class AccessKeyUser : DatabaseEntity
    {
        public int AccessKeyId { get; set; }
        public string Username { get; set; }
        public bool ViewPermitted { get; set; }
        public bool EditPermitted { get; set; }
        public bool DeletePermitted { get; set; }

        public virtual AccessKey AccessKey { get; set; }
    }
}
