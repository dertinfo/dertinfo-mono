namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodIdentifyJudgeSubmissionDto
    {
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string JudgePassword { get; set; }

        public bool AgreeToTermsAndConditions { get; set; }
    }
}
