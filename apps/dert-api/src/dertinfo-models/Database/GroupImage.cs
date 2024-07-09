using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class GroupImage : DatabaseJoin
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public int GroupId { get; set; }
        public bool IsPrimary { get; set; }

        public virtual Group Group { get; set; }
        public virtual Image Image { get; set; }
    }
}
