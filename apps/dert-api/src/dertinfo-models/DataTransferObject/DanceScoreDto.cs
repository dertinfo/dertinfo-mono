using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Models.DataTransferObject
{ 
    public class DanceScoreDto
    { 
        public int DanceId { get; set; }
        public int ScoreCatagoryId { get; set; }
        public string ScoreCatagoryName { get; set; }
        public int  ScoreCatagoryMaxMarks { get; set; }
        public string ScoreCategoryTag { get; set; }
        public int ScoreCategorySortOrder { get; set; }
        public decimal MarkGiven { get; set; }
        public bool Overrun { get; set; } 

    }
} 
