using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class MarkingSheetImage : DatabaseEntity_WithPermissions
    {
        public int ImageId { get; set; }
        public int DanceId { get; set; }

        public virtual Dance Dance { get; set; }
        public virtual Image Image { get; set; }
    }
}
