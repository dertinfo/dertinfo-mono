namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodTeamCollatedResultDto
    {
        public string TeamName { get; set; }
        public decimal SteveMarrisCollatedScore { get; set; }
        public decimal MainCollatedScore { get; set; }
        public decimal MusicCollatedScore { get; set; }
        public decimal SteppingCollatedScore { get; set; }
        public decimal SwordHandlingCollatedScore { get; set; }
        public decimal DanceTechniqueCollatedScore { get; set; }
        public decimal PresentationCollatedScore { get; set; }
        public decimal BuzzCollatedScore { get; set; }
        public decimal CharactersCollatedScore { get; set; }

        /// <summary>
        /// This is the total number of results that have been supplied to create this collated score.
        /// </summary>
        public int NumberOfResults { get; set; }

        /// <summary>
        /// These are the category that the team has been allocated to. For dert of derts there will only be 1.
        /// </summary>
        public string EntryAttribute { get; set; } 
    }
}
