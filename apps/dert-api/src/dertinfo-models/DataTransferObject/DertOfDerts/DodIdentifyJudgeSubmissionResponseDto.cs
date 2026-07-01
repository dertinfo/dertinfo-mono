using System;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodIdentifyJudgeSubmissionResponseDto
    {
        public Guid UserGuid { get; set; }

        public bool IsOfficialJudge { get; set; }
    }
}
