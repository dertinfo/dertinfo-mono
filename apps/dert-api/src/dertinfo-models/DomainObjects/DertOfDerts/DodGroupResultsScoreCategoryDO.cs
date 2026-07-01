using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects.DertOfDerts
{
    public class DodGroupResultsScoreCategoryDO
    {
        public string CategoryName { get; set; }

        public int MaxMarks { get; set; }

        public decimal Score { get; set; }

        public string Comments { get; set; }
    }
}
