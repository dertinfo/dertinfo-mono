using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{
    public class DanceDetailDto
    {
        public int DanceId { get; set; }
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamPictureUrl { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public DateTime? DateScoresEntered { get; set; }
        public bool HasScoresEntered { get; set; }
        public bool HasScoresChecked { get; set; }

        /// <summary>
        /// Locked is to indicate that these dance scores can no longer be changed under any circumstances. 
        /// This is determined by the state of the event and whether the results have been published. 
        /// </summary>
        public bool HasScoresLocked { get; set; }
        public string ScoresEnteredBy { get; set; }
        public bool Overrun { get; set; }

        public ICollection<DanceMarkingSheetDto> DanceMarkingSheets { get; set; }
        public ICollection<DanceScoreDto> DanceScores { get; set; }

    }
}
 