using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects.DertOfDerts
{
    public class DodGroupResultsScoreCardDO
    {
        public int DodResultId { get; set; }

        public int DodSubmissionId { get; set; }

        public List<DodGroupResultsScoreCategoryDO> ScoreCategories { get; set; }

        public string Comments { get; set; }

        public bool IsOfficial { get; set; }

        public bool IsIncluded { get; set; }

        public bool HasOutstandingComplaint { get; set; }
    }
}
