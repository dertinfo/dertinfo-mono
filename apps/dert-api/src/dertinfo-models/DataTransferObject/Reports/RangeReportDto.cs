using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Reports
{
    public class RangeReportDto
    {
        public int JudgeId { get; set; }
        public string JudgeName { get; set; }
        public IList<RangeReportSubRangeDto> ScoreRanges { get; set; }
    }

    public class RangeReportSubRangeDto
    {
        public string RangeName { get; set; }
        public string RangeTag { get; set; }
        public IList<int> RangeValues { get; set; }
    }
}
