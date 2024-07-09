using System.Collections.Generic;

namespace DertInfo.Models.DomainObjects.DertOfDerts
{
    public class DodGroupResultsDO
    {
        public int SubmissionId { get; set; }

        public string GroupName { get; set; }

        public string EmbedLink { get; set; }

        public string EmbedOrigin { get; set; }

        public List<DodGroupResultsScoreCardDO> ScoreCards { get; set; }
    }
}
