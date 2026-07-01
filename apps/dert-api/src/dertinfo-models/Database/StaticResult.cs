using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class StaticResult : DatabaseEntity
    {
        public int EventId { get; set; }
        public int ResultType { get; set; }
        public string HtmlContent { get; set; }
        
    }
}
