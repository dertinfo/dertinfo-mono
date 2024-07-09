using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class BreadcrumbItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public int ObjectId { get; set; }
        public string Label { get; set; }
        public DateTime DateCreated { get; set; }
        public int LineageIndex { get; set; }
        public bool IsTypeRoot { get; set; }
        public string PageUri { get; set; }
    }
}
