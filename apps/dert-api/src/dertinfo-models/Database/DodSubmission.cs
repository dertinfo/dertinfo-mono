using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public class DodSubmission : DatabaseEntity
    {
        public int GroupId { get; set; }
        public string EmbedLink { get; set; }
        public string EmbedOrigin { get; set; }
        public string DertYearFrom { get; set; }
        public string DertVenueFrom { get; set; }
        public int CumulativeNumberOfResults { get; set; }
        public bool IsPremier { get; set; }
        public bool IsChampionship { get; set; }
        public bool IsOpen { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<DodResult> DodResults { get; set; }
    }
}
