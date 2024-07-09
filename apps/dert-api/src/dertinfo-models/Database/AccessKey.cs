using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class AccessKey : DatabaseEntity
    {
        public AccessKey()
        {
            AccessKeyUsers = new HashSet<AccessKeyUser>();
        }

        public Guid AccessKeyRef { get; set; }

        public virtual ICollection<AccessKeyUser> AccessKeyUsers { get; set; }
    }
}
