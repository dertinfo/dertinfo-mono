using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class ScoreSetDto
    {
        public int ScoreSetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> CategoryTags { get; set; }
    }
}
