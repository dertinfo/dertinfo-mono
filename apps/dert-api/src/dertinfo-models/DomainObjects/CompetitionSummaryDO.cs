using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    public class CompetitionSummaryDO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int VenuesCount { get; set; }
        public CompetitionDanceSummaryDO CompetitionDanceSummaryDO { get; set; }
        public int NumberOfCompetitionEntries { get; set; }
        public int NumberOfTicketsSold { get; set; }
        public IEnumerable<CompetitionEntryAttribute> EntryAttributes { get; set; }
        public bool ResultsPublished { get; set; }
        public bool HasBeenPopulated { get; set; }
        public bool HasDancesGenerated { get; set; }
        public bool AllowPopulation { get; set; }
        public bool AllowDanceGeneration { get; set; }
        public bool AllowAdHocDanceAddition { get; set; }
    }
}
