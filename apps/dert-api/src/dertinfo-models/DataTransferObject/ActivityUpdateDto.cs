using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class ActivityUpdateDto
    {

        public int Id { get; set; }
        public int AudienceTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
        public bool PriceTBC { get; set; }
        public bool SoldOut { get; set; }

        // note - these properties are not managed by the update call. Commented here for visibility.
        // public int EventId { get; set; }
        // public int? CompetitionId { get; set; }

    }
}
