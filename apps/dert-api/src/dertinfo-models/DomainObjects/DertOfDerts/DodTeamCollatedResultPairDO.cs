using System.Collections.Generic;

namespace DertInfo.Models.DomainObjects.DertOfDerts
{
    /// <summary>
    /// This class is the collation of the results from both the official judges and the public judges. 
    /// The data in this object results in 2 tables on the client. One showing the official scores and 1 showing the public scores.
    /// </summary>
    public class DodTeamCollatedResultPairDO
    {
        public List<DodTeamCollatedResultDO> CollatedOfficialResults { get; set; }

        public List<DodTeamCollatedResultDO> CollatedPublicResults { get; set; }
    }
}
