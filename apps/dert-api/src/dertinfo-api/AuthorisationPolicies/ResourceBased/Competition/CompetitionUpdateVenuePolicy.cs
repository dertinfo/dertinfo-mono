using DertInfo.Api.AuthorisationPolicies.ResourceBased.Base;
using DertInfo.CrossCutting.Configuration;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    /// <summary>
    /// The event list activity policy assumes that the tickets and prices will not be available until registration has opened.
    /// </summary>
    public class CompetitionUpdateVenuePolicy : HasCompetitionClaimPolicy
    {
        public new static string PolicyName { get { return "CompetitionUpdateVenuePolicy"; } }
    }

    public class CompetitionUpdateVenueRequirement : HasCompetitionClaimRequirement
    {
    }

    public class CompetitionUpdateVenueHandler : HasCompetitionClaimHandler
    {
        public CompetitionUpdateVenueHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }
    }
}
