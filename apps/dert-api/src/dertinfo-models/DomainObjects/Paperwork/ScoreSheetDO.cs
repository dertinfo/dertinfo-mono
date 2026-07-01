using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects.Paperwork
{
    public class ScoreSheetDO
    {
        public Dance Dance { get; set; }
        public Judge Judge { get; set; }
        public Team Team { get; set; }
        public Venue Venue { get; set; }
        public Competition Competition { get; set; }
        public List<CompetitionEntryAttribute> CompetitionEntryAttributes { get; set; }
        public List<ScoreCategory> ScoreCategories { get; set; }
        public Event Event { get; set; }
    }
}
