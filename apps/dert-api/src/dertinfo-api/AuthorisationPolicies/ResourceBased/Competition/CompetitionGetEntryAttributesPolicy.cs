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
    /// <summary>
    /// The event list activity policy assumes that the tickets and prices will not be available until registration has opened.
    /// </summary>
    public class CompetitionGetEntryAttributesPolicy : HasCompetitionClaimPolicy
    {
        public new static string PolicyName { get { return "CompetitionGetEntryAttributesPolicy"; } }
    }

    public class CompetitionGetEntryAttributesRequirement : HasCompetitionClaimRequirement
    {
    }

    public class CompetitionGetEntryAttributesHandler : HasCompetitionClaimHandler
    {
        public CompetitionGetEntryAttributesHandler(IDertInfoConfiguration configuration) : base(configuration)
        { }
    }
}
