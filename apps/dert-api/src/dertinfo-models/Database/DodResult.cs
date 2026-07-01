using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.Database
{
    public class DodResult : DatabaseEntity
    {
        public int SubmissionId { get; set; }

        public int DodUserId { get; set; }

        public decimal MusicScore { get; set; }

        public string MusicComments { get; set; }

        public decimal SteppingScore { get; set; }

        public string SteppingComments { get; set; }

        public decimal SwordHandlingScore { get; set; }

        public string SwordHandlingComments { get; set; }

        public decimal DanceTechniqueScore { get; set; }

        public string DanceTechniqueComments { get; set; }

        public decimal PresentationScore { get; set; }

        public string PresentationComments { get; set; }

        public decimal BuzzScore { get; set; }

        public string BuzzComments { get; set; }

        public decimal CharactersScore { get; set; }

        public string CharactersComments { get; set; }

        public string OverallComments { get; set; }

        public bool IsOfficial { get; set; }

        public bool HasOutstandingComplaint { get; set; }

        public bool IncludeInScores { get; set; }

        public virtual DodSubmission Submission { get; set; }

        public virtual DodUser DodUser { get; set; }

        public virtual ICollection<DodResultComplaint> DodResultComplaints { get; set; }
    }
}
