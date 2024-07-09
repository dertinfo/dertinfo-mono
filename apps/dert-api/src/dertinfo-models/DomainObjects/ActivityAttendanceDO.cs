using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects
{
    /// <summary>
    /// This class represents to attendances to an activity. 
    /// These can be member activity attendances activities or team activity attendances
    /// </summary>
    public class ActivityAttendanceDO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string ActivityName { get; set; }
        public decimal SalesPrice { get; set; }
        public bool SalesPriceTBC { get; set; }

    }
}
