using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.Paperwork
{
    public class ScoreSheetDto
    {
        public string DanceId { get; set; }
        public string JudgeName { get; set; }
        public string TeamName { get; set; }
        public string VenueName { get; set; }
        public string CompetitionName { get; set; }
        public List<CompetitionEntryAttributeDto> CompetitionEntryAttributes { get; set; }
        public List<ScoreCategoryDto> ScoreCategories { get; set; }
        public EventDto Event { get; set; } 
    }
}
