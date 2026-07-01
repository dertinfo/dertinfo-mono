using System.Collections.Generic;

namespace DertInfo.Models.DomainObjects.DertOfDerts
{
    public class DodUserResultsDO
    {
        public int DodUserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<DodGroupResultsScoreCardDO> ScoreCards { get; set; }
    }
}
