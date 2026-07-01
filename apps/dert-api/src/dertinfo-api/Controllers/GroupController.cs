using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.Api.Filters;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.System.Enumerations;
using DertInfo.Services;
using DertInfo.Services.Entity.Groups;
using DertInfo.Services.Entity.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class GroupController : AuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IGroupService _groupService;
        IGroupMemberService _groupMemberService;
        IImageService _imageService;
        IInvoiceService _invoiceService;
        IRegistrationService _registrationService;
        ITeamService _teamService;

        public GroupController(
            IMapper mapper,
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IImageService imageService,
            IInvoiceService invoiceService,
            IDertInfoUser user,
            IRegistrationService registrationService,
            ITeamService teamService
            ) : base(user)
        {
            _mapper = mapper;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _imageService = imageService;
            _invoiceService = invoiceService;
            _registrationService = registrationService;
            _teamService = teamService;
        }

        #region GET

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groups = await _groupService.ListByUser();

            // Perform Auto Map of simple fields
            List<GroupDto> groupDtos = _mapper.Map<List<GroupDto>>(groups);

            // Fill out any more complex mapping items
            foreach (var groupDto in groupDtos)
            {
                try
                {
                    var matchingGroup = groups.First(g => g.Id == groupDto.Id);
                    var groupAccessContext = _groupService.GetUserAccessContext(matchingGroup.Id);
                    groupDto.UserAccessContext = await groupAccessContext;
                }
                catch
                {
                    groupDto.GroupPictureUrl = "";
                }
            }

            return Ok(groupDtos.OrderBy(dto => dto.GroupName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId">Must be present as "groupId" to use group administrator only</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{groupId}")]
        [Authorize(Policy = "GroupMemberPolicy")]
        public async Task<IActionResult> Get(int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupDo = await _groupService.GetOverview(groupId);

            // Perform Auto Map of simple fields
            GroupDto groupDto = _mapper.Map<GroupDto>(groupDo);

            var groupAccessContext = await _groupService.GetUserAccessContext(groupDo.Id);
            groupDto.UserAccessContext = groupAccessContext;

            return Ok(groupDto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId">Must be present as "groupId" to use group administrator only</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{groupId}/overview")]
        [Authorize(Policy = "GroupMemberPolicy")]
        public async Task<IActionResult> GetOverview(int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupOverview = await _groupService.GetOverviewActiveItemsOnly(groupId);

            // Perform Auto Map of simple fields
            GroupOverviewDto groupOverviewDto = _mapper.Map<GroupOverviewDto>(groupOverview);

            return Ok(groupOverviewDto);
        }

        [HttpGet]
        [Route("{groupId}/members")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetMembers(int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupMembers = await _groupService.GetMembers(groupId);

            // Perform Auto Map of simple fields
            List<GroupMemberDto> groupMemberDtos = _mapper.Map<List<GroupMemberDto>>(groupMembers);

            return Ok(groupMemberDtos);
        }

        [HttpGet]
        [Route("{groupId}/invoices")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetInvoices(int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupInvoices = await _invoiceService.GetInvoicesForGroup(groupId);

            // Perform Auto Map of simple fields
            List<GroupInvoiceDto> groupInvoiceDtos = _mapper.Map<List<GroupInvoiceDto>>(groupInvoices);

            foreach (var invoiceDto in groupInvoiceDtos)
            {
                try
                {
                    XmlDocument entryNotesXml = new XmlDocument();
                    entryNotesXml.LoadXml(invoiceDto.InvoiceEntryNotes);
                    string jsonTextEntryNotes = JsonConvert.SerializeXmlNode(entryNotesXml);
                    invoiceDto.InvoiceEntryNotes = jsonTextEntryNotes;

                    XmlDocument invoiceLinesXml = new XmlDocument();
                    invoiceLinesXml.LoadXml(invoiceDto.InvoiceLineItemNotes);
                    string jsonTextInvoiceLines = JsonConvert.SerializeXmlNode(invoiceLinesXml);
                    invoiceDto.InvoiceLineItemNotes = jsonTextInvoiceLines;

                    invoiceDto.HasStructuredNotes = true;
                }
                catch
                {
                    invoiceDto.HasStructuredNotes = false;
                }
            }

            return Ok(groupInvoiceDtos);
        }

        [HttpGet]
        [Route("{groupId}/images")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetImages(int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupImages = await _groupService.GetImages(groupId);

            // Perform Auto Map of simple fields
            List<GroupImageDto> groupImageDtos = _mapper.Map<List<GroupImageDto>>(groupImages);

            return Ok(groupImageDtos);
        }

        [HttpGet]
        [Route("{groupId}/teams")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetTeams(int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupTeams = await _groupService.GetTeams(groupId);

            // Perform Auto Map of simple fields
            List<GroupTeamDto> groupTeamsDtos = _mapper.Map<List<GroupTeamDto>>(groupTeams);

            return Ok(groupTeamsDtos);
        }

        [HttpGet]
        [Route("{groupId}/registrations")]
        [Authorize(Policy = "GroupMemberPolicy")]
        public async Task<IActionResult> GetRegistrations(int groupId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var groupRegistrations = await _groupService.GetRegistrations(groupId);

            // Perform Auto Map of simple fields
            List<GroupRegistrationDto> groupRegistrationDtos = _mapper.Map<List<GroupRegistrationDto>>(groupRegistrations);

            return Ok(groupRegistrationDtos);
        }

        [HttpGet]
        [Route("{groupId}/members/{groupMemberId}")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetMember([FromRoute] int groupId, [FromRoute] int groupMemberId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var groupMember = await _groupMemberService.GetDetail(groupMemberId);

            GroupMemberDetailDto groupMemberDetailDto = _mapper.Map<GroupMemberDetailDto>(groupMember);

            return Ok(groupMemberDetailDto);
        }

        [HttpGet]
        [Route("{groupId}/teams/{teamId}")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetTeam([FromRoute] int groupId, [FromRoute] int teamId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var team = await _teamService.GetDetail(teamId);

            GroupTeamDetailDto groupTeamDetailDto = _mapper.Map<GroupTeamDetailDto>(team);

            return Ok(groupTeamDetailDto);
        }

        #endregion

        #region POST

        [HttpPost]
        [Route("minimal")]
        public async Task<IActionResult> PostMinimal([FromBody] GroupMinimalSubmissionDto groupMinimalSubmission)
        {
            if (!ValidGroupMinimalSubmission(groupMinimalSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            Group myGroup = new Group();
            myGroup.GroupName = groupMinimalSubmission.GroupName;
            myGroup.GroupBio = groupMinimalSubmission.GroupBio;
            myGroup.IsConfigured = false;

            myGroup = await _groupService.CreateMinimal(myGroup);

            GroupDto groupDto = _mapper.Map<GroupDto>(myGroup);

            return Created("api/groups/" + myGroup.Id, groupDto);
        }

        [Obsolete("This was the method used by the web application when creating a group. We no longer use this mechanism to create as we now use CreateMinimal and then configure")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GroupSubmissionDto groupSubmission)
        {
            if (!ValidGroupSubmission(groupSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            byte[] imageBytes = null;
            if (groupSubmission.Base64StringImage != null)
            {
                imageBytes = Convert.FromBase64String(groupSubmission.Base64StringImage);
            }

            // todo - invetigate whether we can cleanly do this via automapper
            Group group = new Group();
            group.GroupName = groupSubmission.GroupName;
            group.GroupBio = groupSubmission.GroupBio;
            group.PrimaryContactName = groupSubmission.PrimaryContactName;
            group.PrimaryContactEmail = groupSubmission.PrimaryContactEmail;
            group.PrimaryContactNumber = groupSubmission.PrimaryContactNumber;
            group.OriginTown = groupSubmission.OriginTown;
            group.OriginPostcode = groupSubmission.OriginPostcode;

            group = await _groupService.Create(group, imageBytes, groupSubmission.UploadImageExtension);

            GroupDto groupDto = _mapper.Map<GroupDto>(group);

            return Created("api/groups/" + group.Id, groupDto);
            // todo - there has to be a better way (other than hardcode) to return the get endpoint as a string based on the routing. 
            //      - there is, it is: CreatedAtRoute()
        }

        /// <summary>
        /// Used to add configuration to a minimal created event
        /// </summary>
        /// <param name="eventConfigurationSubmission"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{groupId}/configure")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> ConfigureGroup([FromRoute] int groupId, [FromBody] GroupConfigurationSubmissionDto groupConfigurationSubmission)
        {
            if (!ValidGroupConfigurationSubmission(groupConfigurationSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            Group myGroup = new Group();
            myGroup.Id = groupId;
            myGroup.GroupBio = groupConfigurationSubmission.GroupBio;
            myGroup.PrimaryContactName = groupConfigurationSubmission.ContactName;
            myGroup.PrimaryContactEmail = groupConfigurationSubmission.ContactEmail;
            myGroup.PrimaryContactNumber = groupConfigurationSubmission.ContactTelephone;
            myGroup.OriginTown = groupConfigurationSubmission.OriginTown;
            myGroup.OriginPostcode = groupConfigurationSubmission.OriginPostcode;
            myGroup.GroupVisibilityType = (GroupVisibilityType)groupConfigurationSubmission.VisibilityType;
            myGroup.IsConfigured = true;
            myGroup.TermsAndConditionsAgreed = groupConfigurationSubmission.AgreeToTermsAndConditions;
            myGroup.TermsAndConditionsAgreedBy = _user.AuthId;

            myGroup = await _groupService.Configure(myGroup);

            GroupDto groupDto = _mapper.Map<GroupDto>(myGroup);

            return Ok(groupDto);
        }


        /// <summary>
        /// Takes a group image submission. 
        /// Stores the image and attaches the image to the group. 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupImageSubmission"></param>
        /// <returns>201: Group Image</returns>
        [HttpPost]
        [RequestFormSizeLimit(valueLengthLimit: 16384)]
        [Route("{groupId}/image")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> Post([FromRoute] int groupId, [FromBody] GroupImageSubmissionDto groupImageSubmission)
        {
            if (!ValidGroupImageSubmission(groupImageSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            byte[] imageBytes = Convert.FromBase64String(groupImageSubmission.Base64StringImage);

            var groupImage = await _groupService.AttachGroupImage(groupImageSubmission.GroupId, imageBytes, groupImageSubmission.UploadImageExtension);

            GroupImageDto groupImageDto = _mapper.Map<GroupImageDto>(groupImage);

            // note - it was considered that we rerturned the location using CreatedAtAction
            //      - however we have no need to get this item seperately at this time
            return Created("", groupImageDto);
        }

        [HttpPost]
        [Route("{groupId}/teamimage")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> AttachTeamImage([FromRoute] int groupId, [FromBody] TeamImageSubmissionDto teamImageSubmission)
        {
            if (!ValidTeamImageSubmission(teamImageSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            TeamImage myTeamImage = new TeamImage();
            myTeamImage.TeamId = teamImageSubmission.TeamId;
            myTeamImage.ImageId = teamImageSubmission.ImageId;
            myTeamImage.IsPrimary = true;


            myTeamImage = await _teamService.AttachImage(myTeamImage);

            TeamImageDto teamImageDto = _mapper.Map<TeamImageDto>(myTeamImage);

            return Ok(teamImageDto);
        }

        [HttpPost]
        [Route("{groupId}/members")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> AddMember([FromRoute] int groupId, [FromBody] GroupMemberSubmissionDto groupMemberSubmission)
        {
            if (!ValidGroupMemberSubmission(groupMemberSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            GroupMember myGroupMember = new GroupMember();
            myGroupMember.GroupId = groupId;
            myGroupMember.Name = groupMemberSubmission.Name;
            myGroupMember.TelephoneNumber = groupMemberSubmission.TelephoneNumber;
            myGroupMember.EmailAddress = groupMemberSubmission.EmailAddress;
            myGroupMember.Facebook = groupMemberSubmission.Facebook;
            myGroupMember.DateOfBirth = groupMemberSubmission.DateOfBirth;
            myGroupMember.DateJoined = groupMemberSubmission.DateJoined;
            myGroupMember.MemberType = (MemberType)groupMemberSubmission.MemberType;

            myGroupMember = await _groupService.CreateMember(myGroupMember);

            GroupMemberDto groupMemberDto = _mapper.Map<GroupMemberDto>(myGroupMember);


            return Created("api/group/" + groupId + "/members/" + myGroupMember.Id, groupMemberDto);
            // todo - there has to be a better way (other than hardcode) to return the get endpoint as a string based on the routing. 
            //      - there is, it is: CreatedAtRoute()
        }

        [HttpPost]
        [Route("{groupId}/teams")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> AddTeam([FromRoute] int groupId, [FromBody] GroupTeamSubmissionDto groupTeamSubmission)
        {
            if (!ValidGroupTeamSubmission(groupTeamSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            Team myTeam = new Team();
            myTeam.GroupId = groupId;
            myTeam.TeamName = groupTeamSubmission.TeamName;
            myTeam.TeamBio = groupTeamSubmission.TeamBio;


            myTeam = await _groupService.CreateTeam(myTeam);

            GroupTeamDto groupTeamDto = _mapper.Map<GroupTeamDto>(myTeam);


            return Created("api/group/" + groupId + "/teams/" + myTeam.Id, groupTeamDto);
            // todo - there has to be a better way (other than hardcode) to return the get endpoint as a string based on the routing. 
            //      - there is, it is: CreatedAtRoute()
        }

        [HttpPost]
        [Route("{groupId}/registrations")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> Post(int groupId, [FromBody] GroupRegistrationSubmissionDto groupRegistrationSubmission)
        {
            if (!ValidGroupRegistrationSubmission(groupRegistrationSubmission)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            Registration myRegistration = new Registration();
            myRegistration.EventId = groupRegistrationSubmission.EventId;
            myRegistration.GroupId = groupId;
            myRegistration.TermsAndConditionsAgreed = groupRegistrationSubmission.AgreeToTermsAndConditions;
            myRegistration.TermsAndConditionsAgreedBy = _user.AuthId;

            myRegistration = await _registrationService.Create(myRegistration);

            GroupRegistrationDto groupRegistrationDto = _mapper.Map<GroupRegistrationDto>(myRegistration);

            return Created("api/group/" + groupId + "/registrations/" + myRegistration.Id, groupRegistrationDto);
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Marks and image for a group as deleted.  
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupImageSubmission"></param>
        /// <returns>201: Group Image</returns>
        [HttpDelete]
        [Route("{groupId}/groupimage/{groupImageId}")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> DeleteGroupImage([FromRoute] int groupId, [FromRoute] int groupImageId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var groupImage = await _groupService.DetachGroupImage(groupId, groupImageId);

            GroupImageDto groupImageDto = _mapper.Map<GroupImageDto>(groupImage);

            if (groupImage != null)
            {
                return Ok(groupImageDto);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpDelete]
        [Route("{groupId}/members/{groupMemberId}")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> RemoveMember([FromRoute] int groupId, [FromRoute] int groupMemberId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var groupMember = await _groupService.RemoveMember(groupMemberId);

            GroupMemberDto groupMemberDto = _mapper.Map<GroupMemberDto>(groupMember);

            if (groupMember != null)
            {
                return Ok(groupMemberDto);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpDelete]
        [Route("{groupId}/teams/{teamId}")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> RemoveTeam([FromRoute] int groupId, [FromRoute] int teamId)
        {
            // todo - somewhere in this stack we need to prevent the user removing the last team on any group. 

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var team = await _groupService.RemoveTeam(teamId);

            GroupTeamDto groupTeamDto = _mapper.Map<GroupTeamDto>(team);

            if (team != null)
            {
                return Ok(groupTeamDto);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpDelete]
        [Route("{groupId}")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> DeleteGroup([FromRoute] int groupId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var group = await _groupService.DeleteGroup(groupId);

            GroupDto groupDto = _mapper.Map<GroupDto>(group);

            if (group != null)
            {
                return Ok(groupDto);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpDelete]
        [Route("{groupId}/abandonconfig")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> AbandonConfiguration([FromRoute] int groupId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var group = await _groupService.DeleteGroup(groupId, true);

            GroupDto groupDto = _mapper.Map<GroupDto>(group);

            if (group != null)
            {
                return Ok(groupDto);
            }
            else
            {
                return NotFound();
            }

        }

        #endregion

        #region PUT

        /// <summary>
        /// We only ever update an group image when we are changing if the image is primary or not
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupImageUpdate"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{groupId}/groupimage/{groupImageId}/setprimary")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> UpdateGroupImagePrimary([FromRoute] int groupId, [FromRoute] int groupImageId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            try
            {
                await _groupService.SetPrimaryGroupImage(groupId, groupImageId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpPut]
        [Route("{groupId}")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> UpdateOverview([FromRoute] int groupId, [FromBody] GroupOverviewUpdateDto groupOverviewUpdate)
        {
            if (!ValidGroupOverviewUpdate(groupOverviewUpdate)) { return BadRequest(this._errorMessage); }
            if (groupId != groupOverviewUpdate.GroupId) { return BadRequest("Submission of an overview update to one group cannot contain details of another."); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            Group myGroup = new Group();
            myGroup.Id = groupId;
            myGroup.GroupName = groupOverviewUpdate.GroupName;
            myGroup.PrimaryContactEmail = groupOverviewUpdate.GroupEmail;
            myGroup.PrimaryContactName = groupOverviewUpdate.ContactName;
            myGroup.PrimaryContactNumber = groupOverviewUpdate.ContactTelephone;
            myGroup.GroupBio = groupOverviewUpdate.GroupBio;
            myGroup.OriginTown = groupOverviewUpdate.OriginTown;
            myGroup.OriginPostcode = groupOverviewUpdate.OriginPostcode;
            myGroup.GroupVisibilityType = (GroupVisibilityType)groupOverviewUpdate.Visibility;

            myGroup = await _groupService.UpdateOverview(myGroup);

            return NoContent();
        }

        [HttpPut]
        [Route("{groupId}/members")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> UpdateMember([FromRoute] int groupId, [FromBody] GroupMemberUpdateDto groupMemberUpdate)
        {
            if (!ValidGroupMemberUpdate(groupMemberUpdate)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            GroupMember myGroupMember = new GroupMember();
            myGroupMember.Id = groupMemberUpdate.GroupMemberId;
            myGroupMember.GroupId = groupId;
            myGroupMember.Name = groupMemberUpdate.Name;
            myGroupMember.TelephoneNumber = groupMemberUpdate.TelephoneNumber;
            myGroupMember.EmailAddress = groupMemberUpdate.EmailAddress;
            myGroupMember.Facebook = groupMemberUpdate.Facebook;
            myGroupMember.DateOfBirth = groupMemberUpdate.DateOfBirth;
            myGroupMember.DateJoined = groupMemberUpdate.DateJoined;
            myGroupMember.MemberType = (MemberType)groupMemberUpdate.MemberType;

            myGroupMember = await _groupMemberService.UpdateMember(myGroupMember);

            GroupMemberDto groupMemberDto = _mapper.Map<GroupMemberDto>(myGroupMember);


            return Ok(groupMemberDto);
        }

        [HttpPut]
        [Route("{groupId}/teams")]
        [Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        public async Task<IActionResult> UpdateTeam([FromRoute] int groupId, [FromBody] GroupTeamUpdateDto teamUpdate)
        {
            if (!ValidGroupTeamUpdate(teamUpdate)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Team myTeam = new Team();
            myTeam.Id = teamUpdate.TeamId;
            myTeam.GroupId = groupId;
            myTeam.TeamName = teamUpdate.TeamName;
            myTeam.TeamBio = teamUpdate.TeamBio;

            myTeam = await _teamService.UpdateTeam(myTeam);

            GroupTeamDto groupTeamDto = _mapper.Map<GroupTeamDto>(myTeam);


            return Ok(groupTeamDto);
        }

        #endregion

        #region Private 

        #endregion

        #region Submission Validation
        private bool ValidGroupMinimalSubmission(GroupMinimalSubmissionDto groupMinimalSubmission)
        {
            if (groupMinimalSubmission == null)
            {
                this._errorMessage = "group minimal submission is null";
                return false;
            }

            if (groupMinimalSubmission.GroupName == string.Empty)
            {
                this._errorMessage = "group name must be supplied";
                return false;
            }

            return true;
        }

        private bool ValidGroupSubmission(GroupSubmissionDto groupSubmission)
        {
            if (groupSubmission == null)
            {
                this._errorMessage = "group submission is null";
                return false;
            }

            if (groupSubmission.GroupName == string.Empty)
            {
                this._errorMessage = "group name must be supplied";
                return false;
            }
            if (groupSubmission.PrimaryContactName == string.Empty)
            {
                this._errorMessage = "group contact name must be supplied";
                return false;
            }
            if (groupSubmission.PrimaryContactEmail == string.Empty)
            {
                this._errorMessage = "group email must be supplied";
                return false;
            }
            if (groupSubmission.Base64StringImage != null && groupSubmission.UploadImageExtension == string.Empty)
            {
                this._errorMessage = "image submission is not permitted without supplying the extension";
                return false;
            }

            return true;
        }

        private bool ValidGroupConfigurationSubmission(GroupConfigurationSubmissionDto groupConfigurationSubmission)
        {
            // todo - this submission should also check nulls etc. 

            if (groupConfigurationSubmission == null)
            {
                this._errorMessage = "group configuration submission is null";
                return false;
            }

            return true;
        }

        private bool ValidGroupOverviewUpdate(GroupOverviewUpdateDto groupOverviewUpdate)
        {
            if (groupOverviewUpdate.GroupName == string.Empty)
            {
                this._errorMessage = "group name must be supplied";
                return false;
            }
            if (groupOverviewUpdate.ContactName == string.Empty)
            {
                this._errorMessage = "group contact name must be supplied";
                return false;
            }
            if (groupOverviewUpdate.GroupEmail == string.Empty)
            {
                this._errorMessage = "group email must be supplied";
                return false;
            }

            return true;
        }

        private bool ValidGroupImageSubmission(GroupImageSubmissionDto groupImageSubmission)
        {
            if (groupImageSubmission.Base64StringImage == null)
            {
                this._errorMessage = "image submission with no image";
                return false;
            }

            if (groupImageSubmission.Base64StringImage != null && groupImageSubmission.UploadImageExtension == string.Empty)
            {
                this._errorMessage = "image submission is not permitted without supplying the extension";
                return false;
            }

            return true;
        }

        private bool ValidGroupMemberSubmission(GroupMemberSubmissionDto groupMemberSubmission)
        {
            if (groupMemberSubmission.Name == string.Empty)
            {
                this._errorMessage = "member name must be supplied";
                return false;
            }

            return true;
        }

        private bool ValidGroupMemberUpdate(GroupMemberUpdateDto groupMemberUpdate)
        {
            if (groupMemberUpdate.Name == string.Empty)
            {
                this._errorMessage = "member name must be supplied";
                return false;
            }

            return true;
        }

        private bool ValidGroupTeamSubmission(GroupTeamSubmissionDto groupTeamSubmission)
        {
            if (groupTeamSubmission.TeamName == string.Empty)
            {
                this._errorMessage = "team name must be supplied";
                return false;
            }

            return true;
        }

        private bool ValidGroupTeamUpdate(GroupTeamUpdateDto groupTeamUpdate)
        {
            if (groupTeamUpdate.TeamName == string.Empty)
            {
                this._errorMessage = "team name must be supplied";
                return false;
            }

            return true;
        }


        private bool ValidTeamImageSubmission(TeamImageSubmissionDto teamImageSubmission)
        {
            if (teamImageSubmission.TeamId == 0)
            {
                this._errorMessage = "team id must be supplied";
                return false;
            }

            if (teamImageSubmission.ImageId == 0)
            {
                this._errorMessage = "image id must be supplied";
                return false;
            }

            return true;
        }

        private bool ValidGroupRegistrationSubmission(GroupRegistrationSubmissionDto groupRegistrationSubmission)
        {
            if (groupRegistrationSubmission == null)
            {
                this._errorMessage = "group registration submission is null";
                return false;
            }

            if (groupRegistrationSubmission.EventId == 0)
            {
                this._errorMessage = "event id must be supplied";
                return false;
            }

            if (groupRegistrationSubmission.AgreeToTermsAndConditions == false)
            {
                this._errorMessage = "the user must agree to the terms and conditions";
                return false;
            }

            return true;
        }

        #endregion
    }
}
