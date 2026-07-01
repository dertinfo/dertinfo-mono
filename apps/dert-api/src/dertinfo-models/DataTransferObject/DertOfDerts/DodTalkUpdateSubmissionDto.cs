using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodTalkUpdateSubmissionDto
    {
        public int TalkId { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string Description { get; set; }

        public DateTime BroadcastDateTime { get; set; }

        public string BroadcastWebLink { get; set; }
    }
}
