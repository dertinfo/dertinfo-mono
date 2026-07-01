using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class SystemSetting : DatabaseEntity
    {
        public string Ref { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
