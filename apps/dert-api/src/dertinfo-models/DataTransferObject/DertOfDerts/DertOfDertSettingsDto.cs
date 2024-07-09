namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DertOfDertSettingsDto
    {
        public bool OpenToPublic { get; set; }

        /// <summary>
        /// Identifies if the results for all are shown on the public part of the site
        /// and if offical scores are shown to the teams.
        /// </summary>
        public bool ResultsPublished { get; set; }

        /// <summary>
        /// Identifies if the public results are forwarded to the teams for inspection. Independent of the offical ones.
        /// </summary>
        public bool PublicResultsForwarded { get; set; }

        /// <summary>
        /// Identifies if the official results are forwarded to the teams for inspection. Independent of the offical ones.
        /// </summary>
        public bool OfficialResultsForwarded { get; set; }

        public string ValidJudgePasswords { get; set; }
    }
}
