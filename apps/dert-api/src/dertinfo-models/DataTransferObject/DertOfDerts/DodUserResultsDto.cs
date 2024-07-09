using DertInfo.Models.DomainObjects.DertOfDerts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodUserResultsDto
    {
        public int DodUserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<DodGroupResultsScoreCardDto> ScoreCards { get; set; }
    }
}
