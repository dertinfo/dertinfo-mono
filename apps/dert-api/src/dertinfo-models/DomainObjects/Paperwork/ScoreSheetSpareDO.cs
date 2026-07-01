using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects.Paperwork
{
    public class ScoreSheetSpareDO
    {
        public Competition Competition { get; set; }
        public List<ScoreCategory> ScoreCategories { get; set; }
        public Event Event { get; set; }
    }
}
