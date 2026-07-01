using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DertInfo.Models.Database
{
    public class DodUser : DatabaseEntity
    {
        [Encrypted]
        public string Name { get; set; }

        [Encrypted]
        public string Email { get; set; }

        public Guid Guid { get; set; }

        public bool TermsAndConditionsAgreed { get; set; }

        public DateTime DateTermsAndConditionsAgreed { get; set; }

        public virtual ICollection<DodResult> DodResults { get; set; }

        public bool IsOfficial { get; set; }

        public bool InterestedInJudging { get; set; }

        public bool ResultsBlocked { get; set; }

        /// <summary>
        /// The user can be recovered for 3 days after creation or last recovery. After this time the information
        /// cannot be retrived. This is to prevent the risk that someone may force the endpoint with guids and emails
        /// until they get a match.
        /// </summary>
        public DateTime RecoveryPermittedUntil { get; set; }
    }
}
