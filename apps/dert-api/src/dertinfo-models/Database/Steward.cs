using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Steward : DatabaseEntity
    {
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Notes { get; set; }
        public int DertYear { get; set; }
    }
}
