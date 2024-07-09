using DertInfo.Api.AuthorisationPolicies.ResourceBased.Base;
using DertInfo.CrossCutting.Configuration;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public class CompetitionAddJudgePolicy: HasCompetitionClaimPolicy
    {
        public new static string PolicyName { get { return "CompetitionAddJudgePolicy"; } }
    }

    public class CompetitionAddJudgeRequirement : HasCompetitionClaimRequirement
    {
    }

    public class CompetitionAddJudgeHandler : HasCompetitionClaimHandler
    {
        public CompetitionAddJudgeHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }
    }
}
