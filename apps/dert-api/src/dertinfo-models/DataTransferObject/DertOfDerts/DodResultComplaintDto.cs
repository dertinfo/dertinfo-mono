using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodResultComplaintDto
    {
        public int Id { get; set; }

        public int DodResultId { get; set; }

        public int DodSubmissionId { get; set; }

        public bool ForScores { get; set; }

        public bool ForComments { get; set; }

        public bool IsResolved { get; set; }

        public bool IsValidated { get; set; }

        public bool IsRejected { get; set; }

        public string Notes { get; set; }

        public string CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public DodGroupResultsScoreCardDto ScoreCard { get; set; }
    }
}
