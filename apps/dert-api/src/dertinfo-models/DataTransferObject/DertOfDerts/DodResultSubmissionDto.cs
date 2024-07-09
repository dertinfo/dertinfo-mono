namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodResultSubmissionDto
    {
        public int SubmissionId { get; set; }

        public string UserGuid { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool InterestedInJudging { get; set; }
        public bool OfficialJudge { get; set; }
        public string OfficialJudgePassword { get; set; }
        public bool AgreeToTermsAndConditions { get; set; }

        #region Score Information

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

        #endregion
    }
}
