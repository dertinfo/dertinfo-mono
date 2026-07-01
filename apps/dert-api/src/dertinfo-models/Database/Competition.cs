using DertInfo.Models.System.Enumerations;
using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Competition : DatabaseEntity_WithPermissions
    {
        public Competition()
        {
            CompetitionEntries = new HashSet<CompetitionEntry>();
            CompetitionEntryAttributes = new HashSet<CompetitionEntryAttribute>();
            CompetitionVenuesJoin = new HashSet<CompetitionVenuesJoin>();
            Dances = new HashSet<Dance>();
            JudgeSlots = new HashSet<JudgeSlot>();
            ScoreCategories = new HashSet<ScoreCategory>();
            ScoreSets = new HashSet<ScoreSet>();
            ResultsPublished = false;
        }

        public string Name { get; set; }
        public string CompetitionDescription { get; set; }
        public decimal TeamEntryFee { get; set; }
        public int EventId { get; set; }
        public int JudgeRequirementPerVenue { get; set; }
        

        public CompetitionFlowState FlowState { get; set; }
        public bool ResultsAreCollated { get; set; }
        public bool ResultsPublished { get; set; }

        /// <summary>
        /// This flag is to allow actions that under normal working conditions would not be permitted. e.g. pre compeition.
        /// </summary>
        public bool InTestingMode { get; set; }

        public bool AllowAdHocDanceAddition { get; set; }

        public bool HasBeenPopulated { get; set; }
        public DateTime? DatePopulated { get; set; }

        public virtual ICollection<CompetitionEntry> CompetitionEntries { get; set; }
        public virtual ICollection<CompetitionEntryAttribute> CompetitionEntryAttributes { get; set; }
        public virtual ICollection<CompetitionVenuesJoin> CompetitionVenuesJoin { get; set; }
        public virtual ICollection<Dance> Dances { get; set; }
        public virtual ICollection<JudgeSlot> JudgeSlots { get; set; }
        public virtual ICollection<ScoreCategory> ScoreCategories { get; set; }
        public virtual ICollection<ScoreSet> ScoreSets { get; set; }
        public virtual ICollection<CompetitionJudge> CompetitionJudges { get; set; }
        public virtual Event Event { get; set; }
    }
}
