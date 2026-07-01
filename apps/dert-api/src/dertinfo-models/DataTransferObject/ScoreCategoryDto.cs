using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class ScoreCategoryDto
    {
        public int ScoreCategoryId { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int MaxMarks { get; set; }
        public int SortOrder { get; set; }
    }
}
