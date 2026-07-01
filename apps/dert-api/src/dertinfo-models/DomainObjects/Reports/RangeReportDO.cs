using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects.Reports
{
    public class RangeReportDO
    {
        public int JudgeId { get; set; }
        public string JudgeName { get; set; }
        public IEnumerable<RangeReportSubRangeDO> ScoreRanges { get; set; }
    }

    public class RangeReportSubRangeDO
    {
        public string RangeName { get; set; }
        public string RangeTag { get; set; }
        public IEnumerable<decimal?> RangeValues { get; set; }
    }
}
