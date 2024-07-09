using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodGroupResultsScoreCategoryDto
    {
        public string CategoryName { get; set; }

        public int MaxMarks { get; set; }

        public decimal Score { get; set; }

        public string Comments { get; set; }
    }
}
