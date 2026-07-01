using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EventInvoiceDto : InvoiceDto
    {
        public decimal CurrentRegistrationTotal { get; set; }
    }
}
