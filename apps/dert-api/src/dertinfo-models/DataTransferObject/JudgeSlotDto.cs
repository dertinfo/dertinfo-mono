using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class JudgeSlotDto
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public int CompetitionId { get; set; }
        public int ScoreSetId { get; set; }
        public int JudgeId { get; set; }
        public string JudgeName { get; set; }
        public string ScoreSetName { get; set; }

    }
}
