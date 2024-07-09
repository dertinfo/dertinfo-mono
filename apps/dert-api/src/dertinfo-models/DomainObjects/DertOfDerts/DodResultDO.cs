using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DomainObjects.DertOfDerts
{
    public class DodResultDO
    {
        public int SubmissionId { get; set; }

        public decimal MusicScore { get; set; }

        public decimal SteppingScore { get; set; }

        public decimal SwordHandlingScore { get; set; }

        public decimal DanceTechniqueScore { get; set; }

        public decimal PresentationScore { get; set; }

        public decimal BuzzScore { get; set; }

        public decimal CharactersScore { get; set; }
    }
}
