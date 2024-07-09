using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class CompetitionSummaryDto
    {
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public int VenuesCount { get; set; }
        public CompetitionDanceSummaryDto CompetitionDanceSummaryDto { get; set; }
        public int NumberOfCompetitionEntries { get; set; }

        public bool HasBeenPopulated { get; set; }
        public bool HasDancesGenerated { get; set; }

        public bool AllowPopulation { get; set; }
        public bool AllowDanceGeneration { get; set; }
        public bool AllowAdHocDanceAddition { get; set; }

        public IEnumerable<CompetitionEntryAttributeDto> EntryAttributes { get; set; }

        public int NumberOfTicketsSold { get; set; }
        public bool ResultsPublished { get; set; }
    }
}
