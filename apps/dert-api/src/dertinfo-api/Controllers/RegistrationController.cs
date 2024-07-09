using AutoMapper;
using DertInfo.Api.AuthorisationPolicies.ResourceBased;
using DertInfo.Api.Controllers.Base;
using DertInfo.Api.Filters;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.DataTransferObject.Emails;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using DertInfo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Services.Entity.Events;
using DertInfo.Services.Entity.Groups;
using DertInfo.Services.Entity.TeamAttendances;
using DertInfo.Services.Entity.MemberAttendance;

namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RegistrationController : ResourceAuthController
    {
        private string _errorMessage = string.Empty;

        private readonly IMapper _mapper;
        private readonly ITeamAttendanceService _teamAttendanceService;
        private readonly IMemberAttendanceService _memberAttendanceService;
        private readonly IEventService _eventService;
        private readonly IGroupService _groupService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPricingService _pricingService;
        private readonly IRegistrationService _registrationService;

        public RegistrationController(
            IAuthorizationService authorizationService,
            ITeamAttendanceService teamAttendanceService,
            IMemberAttendanceService memberAttendanceService,
            IEventService eventService,
            IGroupService groupService,
            IInvoiceService invoiceService,
            IPricingService pricingService,
            IMapper mapper,
            IRegistrationService registrationService,

            IDertInfoUser user
            ) : base(user, authorizationService)
        {
            _mapper = mapper;
            _teamAttendanceService = teamAttendanceService;
            _memberAttendanceService = memberAttendanceService;
            _eventService = eventService;
            _groupService = groupService;
            _invoiceService = invoiceService;
            _pricingService = pricingService;
            _registrationService = registrationService;
        }

        #region GET

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns>
        /// Returns information required to populate the overview for a registration from a group perspective. 
        /// </returns>
        [HttpGet("{registrationId}/group-overview")]
        public async Task<IActionResult> GetGroupOverview(int registrationId)
        {
            var authorisationPolicy = RegistrationViewOverviewsPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var myRegistration = await this._registrationService.GetOverviewForGroup(registrationId);

                GroupRegistrationOverviewDto groupRegistrationOverviewDto = _mapper.Map<GroupRegistrationOverviewDto>(myRegistration);

                groupRegistrationOverviewDto.CurrentTotal = await this._pricingService.GetCurrentPriceForRegistration(myRegistration.Id);
                groupRegistrationOverviewDto.InvoicedTotal = await this._invoiceService.GetLatestInvoicedPriceForRegistration(myRegistration.Id);

                return Ok(groupRegistrationOverviewDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns>
        /// Returns information required to populate the overview for a registration from a event perspective. 
        /// </returns>
        [HttpGet("{registrationId}/event-overview")]
        public async Task<IActionResult> GetEventOverview(int registrationId)
        {
            var authorisationPolicy = RegistrationViewOverviewsPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {


                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var myRegistration = await this._registrationService.GetOverviewForEvent(registrationId);

                EventRegistrationOverviewDto eventRegistrationOverviewDto = _mapper.Map<EventRegistrationOverviewDto>(myRegistration);

                eventRegistrationOverviewDto.CurrentTotal = await this._pricingService.GetCurrentPriceForRegistration(myRegistration.Id);
                eventRegistrationOverviewDto.InvoicedTotal = await this._invoiceService.GetLatestInvoicedPriceForRegistration(myRegistration.Id);

                return Ok(eventRegistrationOverviewDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet("{registrationId}/current-price")]
        public async Task<IActionResult> GetCurrentTotalPrice(int registrationId)
        {
            var authorisationPolicy = RegistrationViewOverviewsPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var myRegistrationCurrentPrice = await this._pricingService.GetCurrentPriceForRegistration(registrationId);

                return Ok(myRegistrationCurrentPrice);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns>
        /// Returns the list of attendances for all individuals attached to a registation (memberAttendnaces). 
        /// This information includes the attendance classification for the attendee. 
        /// todo - this should also be in the form of an activity list for the individual. as there is currently a restriction to 1 ticket per individual.
        /// </returns>
        [HttpGet("{registrationId}/attending-individuals")]
        public async Task<IActionResult> GetAttendingIndividuals(int registrationId)
        {
            var authorisationPolicy = RegistrationViewMemberPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {

                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var memberAttendances = await this._registrationService.ListAttendingIndividuals(registrationId);

                List<MemberAttendanceDto> memberAttendanceDtos = _mapper.Map<List<MemberAttendanceDto>>(memberAttendances);

                return Ok(memberAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns>
        /// Returns the list of attendances for all teams attached to a registation (teamAttendnaces). 
        /// This information includes the activity list for the team
        /// </returns>
        [HttpGet]
        [Route("{registrationId}/attending-teams")]
        public async Task<IActionResult> GetAttendingTeams(int registrationId)
        {
            var authorisationPolicy = RegistrationViewTeamPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {

                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var teamAttendances = await _registrationService.ListAttendingTeams(registrationId);

                List<TeamAttendanceDto> teamAttendanceDtos = _mapper.Map<List<TeamAttendanceDto>>(teamAttendances);

                return Ok(teamAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet]
        [Route("{registrationId}/confirmation-email-data")]
        public async Task<IActionResult> GetConfirmationEmailData(int registrationId)
        {
            var authorisationPolicy = RegistrationConfirmPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var registrationConfirmationEmailData = await _registrationService.GenerateRegistrationConfirmationEmailData(registrationId);

                EmailRegistrationConfirmationDataDto registrationConfirmationEmailDataDto = _mapper.Map<EmailRegistrationConfirmationDataDto>(registrationConfirmationEmailData);

                return Ok(registrationConfirmationEmailDataDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet]
        [Route("{registrationId}/contactinfo/group")]
        public async Task<IActionResult> GetGroupContactInfo(int registrationId)
        {
            var authorisationPolicy = RegistrationGetContactInfoPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var contactDetails = await _groupService.GetContactInfo(registration.GroupId);

                ContactInfoDto contactDetailsDto = _mapper.Map<ContactInfoDto>(contactDetails);

                return Ok(contactDetailsDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet]
        [Route("{registrationId}/contactinfo/event")]
        public async Task<IActionResult> GetEventContactInfo(int registrationId)
        {
            var authorisationPolicy = RegistrationGetContactInfoPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var contactInfo = await _eventService.GetContactInfo(registration.EventId);

                ContactInfoDto contactDetailsDto = _mapper.Map<ContactInfoDto>(contactInfo);

                return Ok(contactDetailsDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet]
        [Route("{registrationId}/individual-activities")]
        public async Task<IActionResult> GetIndividualActivities(int registrationId)
        {
            var authorisationPolicy = RegistrationGetActivitiesPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {

                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var activities = await _eventService.GetIndividualActivities(registration.EventId);

                // Perform Auto Map of simple fields
                List<ActivityDto> activityDtos = _mapper.Map<List<ActivityDto>>(activities);

                return Ok(activityDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpGet]
        [Route("{registrationId}/team-activities")]
        public async Task<IActionResult> GetTeamActivities(int registrationId)
        {
            var authorisationPolicy = RegistrationGetActivitiesPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {

                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var activities = await _eventService.GetTeamActivities(registration.EventId);

                // Perform Auto Map of simple fields
                List<ActivityDto> activityDtos = _mapper.Map<List<ActivityDto>>(activities);

                return Ok(activityDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        #endregion

        #region POST

        /// <summary>
        /// Adding a brand new member through the registration interface
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{registrationId}/attending-individual")]
        public async Task<IActionResult> AttachNewMember([FromRoute] int registrationId, [FromBody] RegistrationMemberAttendanceSubmissionDto submission)
        {
            var authorisationPolicy = RegistrationAddMemberPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                MemberAttendance memberAttendance = null;

                if (submission.GroupMemberId > 0)
                {
                    memberAttendance = await this._registrationService.AddExistingMember(registration, submission.GroupMemberId);

                    if (memberAttendance == null) { return NotFound(); }
                }
                else
                {
                    GroupMember myGroupMember = new GroupMember();
                    myGroupMember.GroupId = registration.GroupId;
                    myGroupMember.Name = submission.GroupMemberSubmission.Name;
                    myGroupMember.TelephoneNumber = submission.GroupMemberSubmission.TelephoneNumber;
                    myGroupMember.EmailAddress = submission.GroupMemberSubmission.EmailAddress;
                    myGroupMember.Facebook = submission.GroupMemberSubmission.Facebook;
                    myGroupMember.DateOfBirth = submission.GroupMemberSubmission.DateOfBirth;
                    myGroupMember.DateJoined = submission.GroupMemberSubmission.DateJoined;
                    myGroupMember.MemberType = (MemberType)submission.GroupMemberSubmission.MemberType;

                    memberAttendance = await this._registrationService.AddNewMember(registration, myGroupMember);
                }

                MemberAttendanceDto memberAttendanceDto = _mapper.Map<MemberAttendanceDto>(memberAttendance);

                return Created("api/registration/" + registrationId + "/attending-individual/" + memberAttendance.Id, memberAttendanceDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{registrationId}/attending-individual/{memberAttendanceId}/activities")]
        public async Task<IActionResult> AddActivitiesToMemberAttendnace([FromRoute] int registrationId, [FromRoute] int memberAttendanceId, [FromBody] RegistrationMemberAttendanceAttachActivitiesSubmissionDto attachSubmission)
        {
            var authorisationPolicy = AttendanceChangeActivitiesPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                IEnumerable<ActivityMemberAttendance> activityMemberAttendances = await this._memberAttendanceService.AddMemberActivities(memberAttendanceId, attachSubmission.ActivityIds);

                IEnumerable<ActivityMemberAttendanceDto> memberAttendanceDtos = _mapper.Map<IEnumerable<ActivityMemberAttendanceDto>>(activityMemberAttendances);

                return Created("api/registration/" + registrationId + "/attending-individuals/" + memberAttendanceId + "/activities", memberAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{registrationId}/attending-individual/{memberAttendanceId}/activities/delete")]
        public async Task<IActionResult> RemoveActivitiesFromMemberAttendnace([FromRoute] int registrationId, [FromRoute] int memberAttendanceId, [FromBody] RegistrationMemberAttendanceDetachActivitiesSubmissionDto detachSubmission)
        {
            var authorisationPolicy = AttendanceChangeActivitiesPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var activityMemberAttendanceIds = detachSubmission.ActivityMemberAttendanceIds;

                IEnumerable<ActivityMemberAttendance> activityMemberAttendances = await this._memberAttendanceService.RemoveMemberAttendanceActivities(activityMemberAttendanceIds);

                IEnumerable<ActivityMemberAttendanceDto> activityMemberAttendanceDtos = _mapper.Map<IEnumerable<ActivityMemberAttendanceDto>>(activityMemberAttendances);

                return Ok(activityMemberAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{registrationId}/attending-team")]
        public async Task<IActionResult> AttachNewTeam([FromRoute] int registrationId, [FromBody] RegistrationTeamAttendanceSubmissionDto submission)
        {
            var authorisationPolicy = RegistrationAddTeamPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                TeamAttendance teamAttendance = null;

                if (submission.TeamId > 0)
                {
                    teamAttendance = await this._registrationService.AddExistingTeam(registration, submission.TeamId);

                    if (teamAttendance == null) { return NotFound(); }
                }
                else
                {
                    Team myTeam = new Team();
                    myTeam.GroupId = registration.GroupId;
                    myTeam.TeamName = submission.GroupTeamSubmission.TeamName;
                    myTeam.TeamBio = submission.GroupTeamSubmission.TeamBio;

                    teamAttendance = await this._registrationService.AddNewTeam(registration, myTeam);
                }

                TeamAttendanceDto teamAttendanceDto = _mapper.Map<TeamAttendanceDto>(teamAttendance);

                return Created("api/registration/" + registrationId + "/attending-team/" + teamAttendance.Id, teamAttendanceDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{registrationId}/attending-team/{teamAttendanceId}/activities")]
        public async Task<IActionResult> AddActivitiesToTeamAttendnace([FromRoute] int registrationId, [FromRoute] int teamAttendanceId, [FromBody] RegistrationTeamAttendanceAttachActivitiesSubmissionDto attachSubmission)
        {
            var authorisationPolicy = AttendanceChangeActivitiesPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                IEnumerable<ActivityTeamAttendance> activityTeamAttendances = await this._teamAttendanceService.AddTeamActivities(teamAttendanceId, attachSubmission.ActivityIds);

                IEnumerable<ActivityTeamAttendanceDto> teamAttendanceDtos = _mapper.Map<IEnumerable<ActivityTeamAttendanceDto>>(activityTeamAttendances);

                return Created("api/registration/" + registrationId + "/attending-individuals/" + teamAttendanceId + "/activities", teamAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{registrationId}/attending-team/{teamAttendanceId}/activities/delete")]
        public async Task<IActionResult> RemoveActivitiesFromTeamAttendnace([FromRoute] int registrationId, [FromRoute] int teamAttendanceId, [FromBody] RegistrationTeamAttendanceDetachActivitiesSubmissionDto detachSubmission)
        {
            var authorisationPolicy = AttendanceChangeActivitiesPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var activityTeamAttendanceIds = detachSubmission.ActivityTeamAttendanceIds;

                IEnumerable<ActivityTeamAttendance> activityTeamAttendances = await this._teamAttendanceService.RemoveTeamAttendanceActivities(activityTeamAttendanceIds);

                IEnumerable<ActivityTeamAttendanceDto> activityTeamAttendanceDtos = _mapper.Map<IEnumerable<ActivityTeamAttendanceDto>>(activityTeamAttendances);

                return Ok(activityTeamAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        /// <summary>
        /// Add a set of existing members through the registation interface
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{registrationId}/attending-individuals")]
        public async Task<IActionResult> AttachExistingMembers([FromRoute] int registrationId, [FromBody] IEnumerable<RegistrationMemberAttendanceSubmissionDto> currentMembersSubmission)
        {
            var authorisationPolicy = RegistrationAddMemberPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var memberIds = currentMembersSubmission.Select(cms => cms.GroupMemberId).ToList();

                IEnumerable<MemberAttendance> memberAttendances = await this._registrationService.AddExistingMembers(registration, memberIds);

                IEnumerable<MemberAttendanceDto> memberAttendanceDtos = _mapper.Map<IEnumerable<MemberAttendanceDto>>(memberAttendances);

                return Created("api/registration/" + registrationId + "/attending-individuals/", memberAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPost]
        [Route("{registrationId}/attending-individuals/delete")]
        public async Task<IActionResult> DeleteExistingMembers([FromRoute] int registrationId, [FromBody] RegistrationMemberAttendanceDeleteSubmissionDto deleteMemberAttendancesSubmission)
        {
            // Do not process if there is nothing to process
            // todo - this could be done with a batch guard.
            if (deleteMemberAttendancesSubmission == null || deleteMemberAttendancesSubmission.MemberAttendanceIds.Length == 0)
            {
                return Ok(new List<MemberAttendanceDto>());
            }

            var authorisationPolicy = RegistrationAddMemberPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var memberAttendanceIds = deleteMemberAttendancesSubmission.MemberAttendanceIds;

                IEnumerable<MemberAttendance> memberAttendances = await this._registrationService.RemoveMemberAttendances(registration, memberAttendanceIds);

                IEnumerable<MemberAttendanceDto> memberAttendanceDtos = _mapper.Map<IEnumerable<MemberAttendanceDto>>(memberAttendances);

                return Ok(memberAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        /// <summary>
        /// Add a set of exisiting teams to the registration
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="currentTeamSubmission"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{registrationId}/attending-teams")]
        public async Task<IActionResult> AttachExistingTeams([FromRoute] int registrationId, [FromBody] IEnumerable<RegistrationTeamAttendanceSubmissionDto> existingTeamSubmission)
        {
            var authorisationPolicy = RegistrationAddTeamPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var teamIds = existingTeamSubmission.Select(cms => cms.TeamId);

                IEnumerable<TeamAttendance> teamAttendances = await this._registrationService.AddExistingTeams(registration, teamIds);

                IEnumerable<TeamAttendanceDto> teamAttendanceDtos = _mapper.Map<IEnumerable<TeamAttendanceDto>>(teamAttendances);

                return Created("api/registration/" + registrationId + "/attending-teams/", teamAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }


        [HttpPost]
        [Route("{registrationId}/attending-teams/delete")]
        public async Task<IActionResult> DeleteExistingTeams([FromRoute] int registrationId, [FromBody] RegistrationTeamAttendanceDeleteSubmissionDto deleteTeamAttendancesSubmission)
        {
            var authorisationPolicy = RegistrationAddTeamPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                var teamAttendanceIds = deleteTeamAttendancesSubmission.TeamAttendanceIds;

                IEnumerable<TeamAttendance> teamAttendances = await this._registrationService.RemoveTeamAttendances(registration, teamAttendanceIds);

                IEnumerable<TeamAttendanceDto> teamAttendanceDtos = _mapper.Map<IEnumerable<TeamAttendanceDto>>(teamAttendances);

                return Ok(teamAttendanceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }


        #endregion

        #region DELETE

        /// <summary>
        /// Allows removal of a memberAttendance from a registration by the member id
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="groupMemberId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{registrationId}/attending-individual/{memberAttendanceId}")]
        public async Task<IActionResult> DetachExistingMember([FromRoute] int registrationId, [FromRoute] int memberAttendanceId)
        {
            var authorisationPolicy = RegistrationRemoveMemberPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                MemberAttendance memberAttendance = await this._registrationService.RemoveMemberAttendance(registration, memberAttendanceId);

                if (memberAttendance == null) { return NotFound(); }

                MemberAttendanceDto memberAttendanceDto = _mapper.Map<MemberAttendanceDto>(memberAttendance);
                return Ok(memberAttendanceDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        /// <summary>
        /// Allows removal of a team from the registration by the team id
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{registrationId}/attending-team/{teamAttendanceId}")]
        public async Task<IActionResult> DetachExistingTeam([FromRoute] int registrationId, [FromRoute] int teamAttendanceId)
        {
            var authorisationPolicy = RegistrationRemoveTeamPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                TeamAttendance teamAttendance = await this._registrationService.RemoveTeamAttendance(registration, teamAttendanceId);

                if (teamAttendance == null) { return NotFound(); }

                TeamAttendanceDto teamAttendanceDto = _mapper.Map<TeamAttendanceDto>(teamAttendance);
                return Ok(teamAttendanceDto);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        #endregion

        #region PUT

        [HttpPut]
        [Route("{registrationId}/submit")]
        public async Task<IActionResult> Submit([FromRoute] int registrationId, [FromBody] RegistrationSubmitSubmissionDto submission)
        {
            var authorisationPolicy = RegistrationSubmitPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                await this._registrationService.AcceptSubmission(registration);

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        [HttpPut]
        [Route("{registrationId}/confirm")]
        public async Task<IActionResult> Confirm([FromRoute] int registrationId, [FromBody] RegistrationConfirmSubmissionDto submission)
        {
            var authorisationPolicy = RegistrationConfirmPolicy.PolicyName;

            Registration registration = await this._registrationService.GetForAuthorization(registrationId);

            if (registration == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, registration))
            {
                base.ExtractUser(); //Fill the scoped injected IDertInfoUser

                await this._registrationService.ConfirmSubmission(registration);

                return NoContent();
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }

        #endregion

        #region Submission Validation
        private bool ValidEventMinimalSubmission(EventMinimalSubmissionDto eventMinimalSubmission)
        {
            if (eventMinimalSubmission == null)
            {
                this._errorMessage = "event minimal submission is null";
                return false;
            }

            if (eventMinimalSubmission.EventName == string.Empty)
            {
                this._errorMessage = "event name must be supplied";
                return false;
            }

            return true;
        }

        #endregion
    }
}
