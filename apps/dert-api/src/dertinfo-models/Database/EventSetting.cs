using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class EventSetting : DatabaseEntity_WithPermissions
    {
        public int EventId { get; set; }
        public string Ref { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual Event Event { get; set; }
    }
}
