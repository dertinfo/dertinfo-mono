using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodBlockUserSubmissionDto
    {
        public int DodUserId { get; set; }

        public bool Block { get; set; }

        public bool UnBlock { get; set; }
    }
}
