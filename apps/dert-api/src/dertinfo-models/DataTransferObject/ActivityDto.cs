using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public int AudienceTypeId { get; set; }
        public int EventId { get; set; }
        public int? CompetitionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool PriceTBC { get; set; }
        public decimal Price { get; set; }
        public bool SoldOut { get; set; }
    }
}
