using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class EmailTemplate : DatabaseEntity_WithPermissions
    {
        public int EventId { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string TemplateRef { get; set; }

        public virtual Event Event { get; set; }
    }
}
