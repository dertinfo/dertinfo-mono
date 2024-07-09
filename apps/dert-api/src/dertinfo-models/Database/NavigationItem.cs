using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class NavigationItem 
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string MinimumRequiredRole { get; set; }
        public int NavigationItemSpecialRef { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
