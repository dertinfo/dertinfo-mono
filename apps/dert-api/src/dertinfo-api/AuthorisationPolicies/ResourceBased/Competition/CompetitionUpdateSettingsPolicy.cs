using DertInfo.Api.AuthorisationPolicies.ResourceBased.Base;
using DertInfo.CrossCutting.Configuration;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    /// <summary>
    /// The event list activity policy assumes that the tickets and prices will not be available until registration has opened.
    /// </summary>
    public class CompetitionUpdateSettingsPolicy : HasCompetitionClaimPolicy
    {
        public new static string PolicyName { get { return "CompetitionUpdateSettingsPolicy"; } }
    }

    public class CompetitionUpdateSettingsRequirement : HasCompetitionClaimRequirement
    {
    }

    public class CompetitionUpdateSettingsHandler : HasCompetitionClaimHandler
    {
        public CompetitionUpdateSettingsHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }
    }
}
