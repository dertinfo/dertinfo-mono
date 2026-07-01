using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodResultComplaintSubmissionDto
    {
        public int ResultId { get; set; }

        public bool ForScores { get; set; }

        public bool ForComments { get; set; }

        public string Notes { get; set; }
    }
}
