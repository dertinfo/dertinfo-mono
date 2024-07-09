using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodGroupResultsScoreCardDto
    {
        public int DodResultId { get; set; }

        public int DodSubmissionId { get; set; }

        public List<DodGroupResultsScoreCategoryDto> ScoreCategories { get; set; }

        public string Comments { get; set; }

        public bool IsOfficial { get; set; }
    }
}
