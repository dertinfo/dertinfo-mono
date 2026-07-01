using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class CompetitionLookupDto
    {
        public List<CompetitionLookupEventDto> Events { get; set; }
    }

    public class CompetitionLookupEventDto
    {
        public string EventName { get; set; }
        public List<CompetitionLookupCompetitionDto> Competitions { get; set; }
    }

    public class CompetitionLookupCompetitionDto
    {
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; }
        public List<CompetitionLookupResultTypeDto> ResultTypes { get; set; }
    }

    public class CompetitionLookupResultTypeDto {
        public string ResultTypeKey { get; set; }
        public string ResultTypeName { get; set; }
    } 

}
