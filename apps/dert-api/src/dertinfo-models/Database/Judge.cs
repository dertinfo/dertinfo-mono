using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Judge : DatabaseEntity_WithPermissions
    {
        public Judge()
        {
            EventJudges = new HashSet<EventJudge>();
            JudgeSlots = new HashSet<JudgeSlot>();
        }

        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        public virtual ICollection<EventJudge> EventJudges { get; set; }
        public virtual ICollection<CompetitionJudge> CompetitionJudges { get; set; }
        public virtual ICollection<JudgeSlot> JudgeSlots { get; set; }
    }
}
