using System.Collections.Generic;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodTeamCollatedResultPairDto
    {
        public List<DodTeamCollatedResultDto> CollatedOfficialResults { get; set; }
        public List<DodTeamCollatedResultDto> CollatedPublicResults { get; set; } 
    }
}
