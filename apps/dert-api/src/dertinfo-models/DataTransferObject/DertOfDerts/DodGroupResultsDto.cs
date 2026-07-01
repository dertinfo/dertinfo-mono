using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodGroupResultsDto
    {
        public int SubmissionId { get; set; }

        public string GroupName { get; set; }

        public string EmbedLink { get; set; }

        public string EmbedOrigin { get; set; }

        public List<DodGroupResultsScoreCardDto> ScoreCards { get; set; }
    }
}
