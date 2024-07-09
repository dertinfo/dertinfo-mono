namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodSubmissionDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupPictureUrl { get; set; }
        public string EmbedLink { get; set; }
        public string EmbedOrigin { get; set; }
        public string DertYearFrom { get; set; }
        public string DertVenueFrom { get; set; }
        public bool HasAnyResults { get; set; }
        public int NumberOfResults { get; set; }
        public bool IsPremier { get; set; }
        public bool IsChampionship { get; set; }
        public bool IsOpen { get; set; }
    }
}
