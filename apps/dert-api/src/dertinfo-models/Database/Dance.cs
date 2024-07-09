using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Dance: DatabaseEntity_WithPermissions
    {
        public Dance()
        {
            DanceScores = new HashSet<DanceScore>();
            MarkingSheetImages = new HashSet<MarkingSheetImage>();
            MarkingSheets = new HashSet<MarkingSheet>();
        }

        public int VenueId { get; set; }
        public int CompetitionId { get; set; }
        public int DertYear { get; set; }
        public bool HasScoresEntered { get; set; }
        public bool HasScoresChecked { get; set; }
        public DateTime? DateScoresEntered { get; set; }
        public string ScoresEnteredBy { get; set; }
        public int TeamAttendanceId { get; set; }
        public bool Overrun { get; set; }

        public virtual ICollection<DanceScore> DanceScores { get; set; }
        public virtual ICollection<MarkingSheetImage> MarkingSheetImages { get; set; }
        public virtual ICollection<MarkingSheet> MarkingSheets { get; set; }
        public virtual Competition Competition { get; set; }
        public virtual TeamAttendance TeamAttendance { get; set; }
        public virtual Venue Venue { get; set; }

    }
}
