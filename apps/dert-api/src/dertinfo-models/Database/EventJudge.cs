using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class EventJudge : DatabaseEntity
    {
        public int EventId { get; set; }
        public int JudgeId { get; set; }

        public virtual Event Event { get; set; }
        public virtual Judge Judge { get; set; }
    }
}
