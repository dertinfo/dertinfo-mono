using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject.Notifications;
using DertInfo.Services.Entity.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DertInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : AuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        INotificationService _notificationService;

        public NotificationController(
            IMapper mapper,
            INotificationService notificationService,
            IDertInfoUser user
            ) : base(user)
        {
            _mapper = mapper;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> NotificationPanelOpened()
        {
            base.ExtractUser();

            var summaryNotificationDOs = await this._notificationService.GetNotificationSummariesForUser();

            List<NotificationSummaryDto> summaryNotificationDtos = _mapper.Map<List<NotificationSummaryDto>>(summaryNotificationDOs);

            return Ok(summaryNotificationDtos.OrderByDescending(n => n.DateCreated));
        }

        [HttpGet]
        [Route("list")]
        [Authorize(Policy = "SuperAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetAllNotifications()
        {
            base.ExtractUser();

            var notifications = await this._notificationService.GetNotificationListForAdmin();

            List<NotificationMessageDto> notificationDtos = _mapper.Map<List<NotificationMessageDto>>(notifications);

            return Ok(notificationDtos.OrderByDescending(n => n.DateCreated));
        }

        [HttpGet]
        [Route("check")]
        public async Task<IActionResult> CheckForNewNotificationsForUser()
        {
            base.ExtractUser();

            var messageThumbnailInfoDo = await this._notificationService.CheckForNewNotificationsForUser();

            NotificationThumbnailInfoDto notificationThumbnailInfoDto = _mapper.Map<NotificationThumbnailInfoDto>(messageThumbnailInfoDo);

            return Ok(notificationThumbnailInfoDto);
        }

        /// <summary>
        /// [System Administrator Only] Creates a new notification for the system to distribute.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "SuperAdministratorOnlyPolicy")]
        public async Task<IActionResult> Create([FromBody] NotificationMessageSubmissionDto notificationMessageSubmissionDto) {

            if (!ValidNotificationMessageSubmission(notificationMessageSubmissionDto)) { return BadRequest(this._errorMessage); }

            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            // todo - invetigate whether we can cleanly do this via automapper
            NotificationMessage myNotification = new NotificationMessage();
            myNotification.MessageHeader = notificationMessageSubmissionDto.MessageHeader;
            myNotification.MessageSummary = notificationMessageSubmissionDto.MessageSummary;
            myNotification.MessageBody = notificationMessageSubmissionDto.MessageBody;
            myNotification.HasDetails = notificationMessageSubmissionDto.HasDetails;
            myNotification.RequiresOpening = notificationMessageSubmissionDto.RequiresOpening;
            myNotification.RequiresAcknowledgement = notificationMessageSubmissionDto.RequiresAcknowledgement;
            myNotification.BlocksUser = notificationMessageSubmissionDto.BlocksUser;
            myNotification.Severity = notificationMessageSubmissionDto.Severity;


            myNotification = await _notificationService.CreateNotificationMessage(myNotification);

            NotificationMessageDto notificationMessageDto = _mapper.Map<NotificationMessageDto>(myNotification);

            // note - it was considered that we rerturned the location using CreatedAtAction
            //      - however we have no need to get this item seperately at this time
            return Created("", notificationMessageDto);

        }

        [HttpGet]
        [Route("{notificationAudienceLogId}/detail")]
        public async Task<IActionResult> GetNotificationDetail(int notificationAudienceLogId)
        {
            base.ExtractUser();

            var notificationDetailDo = await this._notificationService.GetNotificationDetailForUser(notificationAudienceLogId);

            NotificationDetailDto notificationDetailDto = _mapper.Map<NotificationDetailDto>(notificationDetailDo);

            return Ok(notificationDetailDto);
        }

        [HttpPut]
        [Route("{notificationAudienceLogId}/dismiss")]
        public async Task<IActionResult> DismissNotification(int notificationAudienceLogId)
        {
            base.ExtractUser();

            await this._notificationService.DismissNotificationByUser(notificationAudienceLogId);

            return Ok();
        }

        [HttpPut]
        [Route("{notificationAudienceLogId}/acknowledge")]
        public async Task<IActionResult> AcknowledgeNotification(int notificationAudienceLogId)
        {
            base.ExtractUser();

            await this._notificationService.AcknowledgeNotification(notificationAudienceLogId);

            return Ok();
        }

        [HttpPut]
        [Route("{notificationAudienceLogId}/clear")]
        public async Task<IActionResult> ClearNotification(int notificationAudienceLogId)
        {
            base.ExtractUser();

            await this._notificationService.ClearNotification(notificationAudienceLogId);

            return Ok();
        }

        /// <summary>
        /// Soft deletes the notification and updates any bound audience messages.
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{notificationMessageId}")]
        [Authorize(Policy = "SuperAdministratorOnlyPolicy")]
        public async Task<IActionResult> DeleteNotificationMessage([FromRoute] int notificationMessageId)
        {
            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            var notificationMessage = await _notificationService.DeleteNotificationMessage(notificationMessageId);

            NotificationMessageDto notificationMessageDto = _mapper.Map<NotificationMessageDto>(notificationMessage);

            if (notificationMessageDto != null)
            {
                return Ok(notificationMessageDto);
            }
            else
            {
                return NotFound();
            }
        }

        private bool ValidNotificationMessageSubmission(NotificationMessageSubmissionDto notificationMessageSubmission)
        {
            if (notificationMessageSubmission == null)
            {
                this._errorMessage = "notification message submission is null";
                return false;
            }

            if (notificationMessageSubmission.MessageHeader == string.Empty)
            {
                this._errorMessage = "a message header must be supplied";
                return false;
            }

            if (notificationMessageSubmission.MessageSummary == string.Empty)
            {
                this._errorMessage = "a message summary must be supplied";
                return false;
            }

            return true;
        }
    }
}
