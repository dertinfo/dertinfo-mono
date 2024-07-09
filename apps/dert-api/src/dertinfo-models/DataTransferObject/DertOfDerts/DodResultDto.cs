using System;

namespace DertInfo.Models.DataTransferObject.DertOfDerts
{
    public class DodResultDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Additional loading of the response in the case of the inital submission to inform the user of thier guid. Will be empty in all other scenarios.
        /// </summary>
        /// <remarks>
        /// When the user submits the results we need to send them back their user guid in order that we are not sending PII over the wire lots of times.
        /// As we respond the the post with this object we've loaded this object with the property. 
        /// It would be reasonable to create a seperate object for the response for the create but this would bloat the models.
        /// </remarks>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// Identified if the user is recognosed as an official judge
        /// </summary>
        public bool IsOfficialJudge { get; set; }

        public int SubmissionId { get; set; }

        public string DodUserId { get; set; }

        public decimal MusicScore { get; set; }

        public string MusicComments { get; set; }

        public decimal SteppingScore { get; set; }

        public string SteppingComments { get; set; }

        public decimal SwordHandlingScore { get; set; }

        public string SwordHandlingComments { get; set; }

        public decimal DanceTechniqueScore { get; set; }

        public string DanceTechniqueComments { get; set; }

        public decimal PresentationScore { get; set; }

        public string PresentationComments { get; set; }

        public decimal BuzzScore { get; set; }

        public string BuzzComments { get; set; }

        public decimal CharactersScore { get; set; }

        public string CharactersComments { get; set; }

        public string OverallComments { get; set; }
    }
}
