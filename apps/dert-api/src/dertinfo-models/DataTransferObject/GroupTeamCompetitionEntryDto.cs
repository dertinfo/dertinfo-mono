using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class GroupTeamCompetitionEntryDto
    {
        public int CompetitionEntryId { get; set; }
        public int TeamId { get; set; }
        public int GroupId { get; set; }
        public string TeamName { get; set; }
        public string TeamBio { get; set; }
        public string TeamPictureUrl { get; set; }
        public bool ShowShowcase { get; set; }
        public bool HasBeenAddedToCompetition { get; set; }
        public IEnumerable<CompetitionEntryAttributeDto> EntryAttributes { get; set; }
    }
}
