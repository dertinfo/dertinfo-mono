using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Results
{
    public class DanceResultDto
    {
        public int DanceId { get; set; }
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamPictureUrl { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public bool HasScoresEntered { get; set; }
        public bool HasScoresChecked { get; set; }
        public string ScoresEnteredBy { get; set; }
        public bool Overrun { get; set; }

        public ICollection<DanceMarkingSheetDto> DanceMarkingSheets { get; set; }
        public ICollection<DanceScoreDto> DanceScores { get; set; }
    }
}
