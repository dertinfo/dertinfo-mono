using DertInfo.Api.AuthorisationPolicies.ResourceBased.Base;
using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.AuthorisationPolicies.ResourceBased
{
    public class CompetitionGetFullCollatedResultsPolicy: HasCompetitionClaimPolicy
    {
        public new static string PolicyName { get { return "CompetitionGetFullCollatedResultsPolicy"; } }
    }

    public class CompetitionGetFullCollatedResultsRequirement : HasCompetitionClaimRequirement
    {
    }

    public class CompetitionGetFullCollatedResultsHandler : HasCompetitionClaimHandler
    {
        public CompetitionGetFullCollatedResultsHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }
    }
}
