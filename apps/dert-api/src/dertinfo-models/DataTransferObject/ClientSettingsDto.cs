using System;

namespace DertInfo.Models.DataTransferObject
{
    public class ClientSettingsDto
    {
        public bool DodOpenToPublic { get; set; }

        public bool DodResultsPublished { get; set; }

        public bool DodPublicResultsForwarded { get; set; }

        public bool DodOfficialResultsForwarded { get; set; }
    }
}
