using DertInfo.Models.System.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Results
{
    public class CompetitionFullResultDto
    {
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public IEnumerable<TeamCollatedFullResultDto> TeamCollatedFullResults { get; set; }

    }
}
