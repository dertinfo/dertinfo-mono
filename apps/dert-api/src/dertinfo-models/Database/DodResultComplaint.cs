namespace DertInfo.Models.Database
{
    public class DodResultComplaint : DatabaseEntity
    {
        public int DodResultId { get; set; }

        public bool ForScores { get; set; }

        public bool ForComments { get; set; }

        public bool IsResolved { get; set; }

        public bool IsValidated { get; set; }

        public bool IsRejected { get; set; }

        public string Notes { get; set; }

        public virtual DodResult DodResult { get; set; }
    }
}
