using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Database
{
    public class CompetitionJudge : DatabaseEntity
    {
        public int CompetitionId { get; set; }
        public int JudgeId { get; set; }

        public virtual Competition Competition { get; set; }
        public virtual Judge Judge { get; set; }
    }
}
