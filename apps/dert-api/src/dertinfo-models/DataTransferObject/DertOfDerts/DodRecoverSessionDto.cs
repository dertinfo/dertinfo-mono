using System.Collections.Generic;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodRecoverSessionDto
    {
        public string UserName { get; set; }
        public bool OfficialJudge { get; set; }
        public List<int> DancesJudged { get; set; }
    }
}
