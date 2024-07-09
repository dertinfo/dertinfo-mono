using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class EventImage : DatabaseJoin
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public int EventId { get; set; }
        public bool IsPrimary { get; set; }

        public virtual Event Event { get; set; }
        public virtual Image Image { get; set; }
    }
}
