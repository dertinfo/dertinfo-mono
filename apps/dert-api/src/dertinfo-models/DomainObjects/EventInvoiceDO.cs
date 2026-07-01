using DertInfo.Models.Database;
using System;

namespace DertInfo.Models.DomainObjects
{
    public class EventInvoiceDO
    {
        public EventInvoiceDO() { }

        public EventInvoiceDO(Invoice invoice)
        {
            this.Id = invoice.Id;
            this.InvoiceCode = invoice.InvoiceCode;
            this.RegistrationId = invoice.RegistrationId;
            this.InvoiceToName = invoice.InvoiceToName;
            this.InvoiceToEmail = invoice.InvoiceToEmail;
            this.InvoiceTotal = invoice.InvoiceTotal;
            this.InvoiceLineItemNotes = invoice.InvoiceLineItemNotes;
            this.InvoiceEntryNotes = invoice.InvoiceEntryNotes;
            this.HasPaid = invoice.HasPaid;
            this.Registration = invoice.Registration;
            this.DateCreated = invoice.DateCreated;
        }

        // Database entity
        public int Id { get; set; }
        public string InvoiceCode { get; set; }
        public int RegistrationId { get; set; }
        public string InvoiceToName { get; set; }
        public string InvoiceToTeamName { get; set; }
        public string InvoiceToEmail { get; set; }
        public decimal InvoiceTotal { get; set; }
        public string InvoiceLineItemNotes { get; set; }
        public string InvoiceEntryNotes { get; set; }

        
        public bool HasPaid { get; set; }
        public Registration Registration { get; set; }

        // Augmented property
        public decimal CurrentRegistrationTotal { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
