using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodJudgeInfoDto
    {
        public int DodUserId { get; set; }
        public string JudgeName { get; set; }
        public string JudgeEmail { get; set; }
        public bool ResultsBlocked { get; set; }
        public int CountToComplete { get; set; }
        public int CountCompleted { get; set; }
        public bool IsOfficial { get; set; }
        public bool InterestedInJudging { get; set; }
    }
}
