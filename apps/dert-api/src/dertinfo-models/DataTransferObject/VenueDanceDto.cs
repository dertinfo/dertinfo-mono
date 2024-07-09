using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class VenueDanceDto
    {

        public VenueDanceDto()
        {
        } 

        public int DanceId { get; set; }
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamPictureUrl { get; set; }
        public bool HasScoresEntered { get; set; }
        public bool HasScoresChecked { get; set; }
        public string ScoresEnteredBy { get; set; }
        
    }
}
