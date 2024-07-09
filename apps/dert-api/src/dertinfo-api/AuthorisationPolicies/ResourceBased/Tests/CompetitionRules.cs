using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased.Tests
{
    public static class CompetitionRules
    {
        public static bool EditingSettingsBlocked(Competition competition)
        {
            return competition.FlowState == Models.System.Enumerations.CompetitionFlowState.Published;
        }

        public static bool EditingVenuesBlocked(Competition compeition)
        {
            return compeition.FlowState == Models.System.Enumerations.CompetitionFlowState.Published;
        }

        public static bool EditingJudgesBlocked(Competition compeition)
        {
            return compeition.FlowState == Models.System.Enumerations.CompetitionFlowState.Published;
        }

        public static bool EditingDancesBlocked(Competition compeition)
        {
            return compeition.FlowState == Models.System.Enumerations.CompetitionFlowState.Published;
        }

        public static bool EditingResultsBlocked(Competition compeition)
        {
            return compeition.FlowState == Models.System.Enumerations.CompetitionFlowState.Published;
        }

        public static bool AddDancePermitted(Competition compeition)
        {
            return compeition.FlowState == CompetitionFlowState.Generated && !compeition.ResultsPublished;
        }
    }
}
