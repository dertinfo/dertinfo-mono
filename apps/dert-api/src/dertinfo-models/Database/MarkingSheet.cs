using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class MarkingSheet : DatabaseEntity_WithPermissions
    {
        public int DanceId { get; set; }
        public string ScoreSheetImageUrl { get; set; }

        public virtual Dance Dance { get; set; }
    }
}
