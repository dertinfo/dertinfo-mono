using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventActivityDto : ActivityDto
    {
        public int AttendanceCount { get; set; }
        public decimal ValueOfSales { get; set; }
    }
}
