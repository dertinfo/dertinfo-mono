using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Spectator : DatabaseEntity
    {
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public int NumberOfAdultTickets { get; set; }
        public int NumberOfAdultConcessionTickets { get; set; }
        public int NumberOfYouthTickets { get; set; }
        public int NumberOfJuniorTickets { get; set; }
        public int NumberOfCampingTickets { get; set; }
        public string Notes { get; set; }
        public int DertYear { get; set; }
    }
}
