using DertInfo.Api.AuthorisationPolicies.ResourceBased.Base;
using DertInfo.CrossCutting.Configuration;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public class PaperworkGetScoreSheetsPolicy : HasCompetitionClaimPolicy
    {
        public new static string PolicyName { get { return "PaperworkGetScoreSheetsPolicy"; } }
    }

    public class PaperworkGetScoreSheetsRequirement : HasCompetitionClaimRequirement
    {
    }

    public class PaperworkGetScoreSheetsHandler : HasCompetitionClaimHandler
    {
        public PaperworkGetScoreSheetsHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }
    }
}
