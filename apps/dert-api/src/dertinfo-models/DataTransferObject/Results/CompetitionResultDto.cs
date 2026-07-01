using DertInfo.Models.System.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Results
{
    public class CompetitionResultDto
    {
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public string ResultType { get; set; }
        public IEnumerable<TeamCollatedResultDto> TeamCollatedResults { get; set; }
        public int[] ScoreCategoryIdsIncluded { get; set; }

    }
}
