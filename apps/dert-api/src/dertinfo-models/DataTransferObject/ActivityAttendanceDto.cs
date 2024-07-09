using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class ActivityAttendanceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string ActivityName { get; set; }
        public decimal SalesPrice { get; set; }
        public bool SalesPriceTBC { get; set; }
    }
}
