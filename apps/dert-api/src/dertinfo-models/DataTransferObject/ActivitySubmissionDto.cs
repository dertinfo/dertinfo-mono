using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class ActivitySubmissionDto
    {
        public int AudienceTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
        public bool PriceTBC { get; set; }
        public bool SoldOut { get; set; }

    }
}
