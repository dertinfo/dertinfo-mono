using DertInfo.Api.AuthorisationPolicies.ResourceBased.Base;
using DertInfo.CrossCutting.Configuration;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    /// <summary>
    /// The event list activity policy assumes that the tickets and prices will not be available until registration has opened.
    /// </summary>
    public class CompetitionUpdateEntrantAttributesPolicy : HasCompetitionClaimPolicy
    {
        public new static string PolicyName { get { return "CompetitionUpdateEntrantAttributesPolicy"; } }
    }

    public class CompetitionUpdateEntrantAttributesRequirement : HasCompetitionClaimRequirement
    {
    }

    public class CompetitionUpdateEntrantAttributesHandler : HasCompetitionClaimHandler
    {
        public CompetitionUpdateEntrantAttributesHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }
    }
}
