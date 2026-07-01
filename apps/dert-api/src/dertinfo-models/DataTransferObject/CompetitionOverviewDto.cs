using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class CompetitionOverviewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CompetitionSummaryDto Summary { get; set; }
    }
}
