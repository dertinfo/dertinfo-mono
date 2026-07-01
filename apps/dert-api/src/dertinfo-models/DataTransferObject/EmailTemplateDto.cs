using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject
{
    public class EmailTemplateDto
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; }
    }
}
