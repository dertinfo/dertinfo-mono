using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class ClientConfigurationDto
    {
        public string AppInsightsTelemetryKey { get; set; }
        public string Auth0Audience { get; set; }
        public string Auth0CallbackUrl { get; set; }
        public string Auth0ClientId { get; set; }
        public string Auth0TenantDomain { get; set; }
    }
}
