using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodTalkDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string Description { get; set; }

        public DateTime RecordedDateTime { get; set; }

        public DateTime BroadcastDateTime { get; set; }

        public string BroadcastWebLink { get; set; }
    }
}
