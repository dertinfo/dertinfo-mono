using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public int RegistrationId { get; set; }
        public string InvoiceToName { get; set; }
        public string InvoiceToTeamName { get; set; }
        public string InvoiceToEmail { get; set; }
        public decimal InvoiceTotal { get; set; }
        public string InvoiceLineItemNotes { get; set; }
        public string InvoiceEntryNotes { get; set; }
        public bool HasPaid { get; set; }

        public string Title { get; set; }
        public string ImageResourceUri { get; set; }

        public bool HasStructuredNotes { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
