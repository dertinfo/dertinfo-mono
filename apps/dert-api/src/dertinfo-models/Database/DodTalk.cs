using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Database
{
    public class DodTalk : DatabaseEntity
    {
        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string Description { get; set; }

        public DateTime BroadcastDateTime { get; set; }

        public string BroadcastWebLink { get; set; }
    }
}
