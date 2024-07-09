using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Results
{
    public class TeamResultDto
    {
        public ICollection<DanceDetailDto> DanceResults { get; set; }
    }
}
