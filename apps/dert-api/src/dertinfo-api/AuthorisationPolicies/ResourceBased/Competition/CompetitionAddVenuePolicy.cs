using DertInfo.Api.AuthorisationPolicies.ResourceBased.Base;
using DertInfo.CrossCutting.Configuration;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    /// <summary>
    /// The event list activity policy assumes that the tickets and prices will not be available until registration has opened.
    /// </summary>
    public class CompetitionAddVenuePolicy : HasCompetitionClaimPolicy
    {
        public new static string PolicyName { get { return "CompetitionAddVenuePolicy"; } }
    }

    public class CompetitionAddVenueRequirement : HasCompetitionClaimRequirement
    {
    }

    public class CompetitionAddVenueHandler : HasCompetitionClaimHandler
    {
        public CompetitionAddVenueHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }
    }
}
