using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class TeamImage : DatabaseJoin
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public int TeamId { get; set; }
        public bool IsPrimary { get; set; }

        public virtual Image Image { get; set; }
        public virtual Team Team { get; set; }
    }
}
