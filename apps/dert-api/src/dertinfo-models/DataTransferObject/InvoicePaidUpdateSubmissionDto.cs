using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class InvoicePaidUpdateSubmissionDto
    {
        public int InvoiceId { get; set; }
        public bool HasPaid { get; set; }
    }
}
