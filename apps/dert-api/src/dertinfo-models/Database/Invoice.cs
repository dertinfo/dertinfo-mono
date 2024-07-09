using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Invoice : DatabaseEntity_WithPermissions
    {
        public string InvoiceCode { get; set; }
        public int RegistrationId { get; set; }
        public string InvoiceToName { get; set; }
        public string InvoiceToTeamName { get; set; }
        public string InvoiceToEmail { get; set; }
        public decimal InvoiceTotal { get; set; }
        public string InvoiceLineItemNotes { get; set; }
        public string InvoiceEntryNotes { get; set; }
        public bool HasPaid { get; set; }

        public virtual Registration Registration { get; set; }
    }
}
