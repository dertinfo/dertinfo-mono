using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodSubmissionUpdateSubmissionDto
    {
        public int SubmissionId { get; set; }
        public int GroupId { get; set; }
        public string EmbedLink { get; set; }
        public string EmbedOrigin { get; set; }
        public string DertYearFrom { get; set; }
        public string DertVenueFrom { get; set; }
        public bool IsPremier { get; set; }
        public bool IsChampionship { get; set; }
        public bool IsOpen { get; set; }
    }
}
