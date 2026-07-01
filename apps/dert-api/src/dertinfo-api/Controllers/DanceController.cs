using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.Api.Filters;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Services;
using DertInfo.Services.Entity.Competitions;
using DertInfo.Services.Entity.Dances;
using DertInfo.Services.Entity.Images;
using DertInfo.Services.Entity.JudgeSlots;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DanceController : AuthController
    {
        IMapper _mapper;
        ICompetitionService _competitionService;
        IDanceService _danceService;
        IImageUtilities _imageUtilities;
        IImageService _imageService;
        IJudgeSlotService _judgeSlotService;

        public DanceController(
            IMapper mapper,
            ICompetitionService competitionService,
            IDanceService danceService,
            IImageUtilities imageUtilities,
            IDertInfoUser user,
            IImageService imageService,
            IJudgeSlotService judgeSlotService
            ) : base(user)
        {
            _mapper = mapper;
            _competitionService = competitionService;
            _judgeSlotService = judgeSlotService;
            _danceService = danceService;
            _imageUtilities = imageUtilities;
            _imageService = imageService;
        }

        //// GET api/venue/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            base.ExtractUser();

            var dance = await _danceService.FindById(id);
            var competition = await _competitionService.FindById(dance.CompetitionId);

            DanceDetailDto danceDetailDto = _mapper.Map<DanceDetailDto>(dance);
            danceDetailDto = await MappingSupport_TeamImage(dance, danceDetailDto);

            // Add this information from the competition to the result.
            danceDetailDto.HasScoresLocked = competition.ResultsPublished;

            return Ok(danceDetailDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DanceResultsSubmissionDto danceScoreSubmission)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            List<DanceScore> danceScores = new List<DanceScore>();

            foreach (var score in danceScoreSubmission.DanceScores)
            {
                DanceScore danceScore = new DanceScore();
                danceScore.DanceId = danceScoreSubmission.DanceId;
                danceScore.ScoreCategoryId = score.ScoreCategoryId;
                danceScore.MarkGiven = score.MarksGiven;

                danceScores.Add(danceScore);
            }

            var dance = await _danceService.UpdateDanceAndScores(danceScoreSubmission.DanceId, danceScores, danceScoreSubmission.Overrun);

            DanceDetailDto danceDetailDto = _mapper.Map<DanceDetailDto>(dance);
            danceDetailDto = await MappingSupport_TeamImage(dance, danceDetailDto);

            return Ok(danceDetailDto);

        }

        // POST api/values
        /// <summary>
        /// Takes a marking sheet submission. 
        /// Stores the image and attaches the image to the dance. 
        /// </summary>
        /// <param name="danceId"></param>
        /// <param name="danceMarkingSheetSubmission"></param>
        /// <returns>201: Marking sheet</returns>
        [HttpPost]
        [RequestFormSizeLimit(valueLengthLimit: 26214400)]
        [Route("{danceId}/markingsheet")]
        public async Task<IActionResult> Post([FromRoute] int danceId, [FromBody] DanceMarkingSheetSubmissionDto danceMarkingSheetSubmission)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            byte[] imageBytes = Convert.FromBase64String(danceMarkingSheetSubmission.Base64StringImage);

            var markingSheet = await _danceService.AttachMarkingSheet(danceMarkingSheetSubmission.DanceId, imageBytes);

            DanceMarkingSheetDto danceMarkingSheetDto = _mapper.Map<DanceMarkingSheetDto>(markingSheet);

            return CreatedAtAction("GetMarkingSheet", new { danceId = danceId, markingSheetId = markingSheet.Id }, danceMarkingSheetDto);

        }

        // DELETE api/values/5
        [HttpDelete]
        [Route("{danceId}/markingsheet/{markingSheetId}")]
        public async Task<IActionResult> Delete(int danceId, int markingSheetId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var dance = await _danceService.FindById(danceId);

            if (dance != null && dance.MarkingSheetImages.Any(ms => ms.Id == markingSheetId))
            {
                var deletedMarkedSheet = await _danceService.DetachMarkingSheet(markingSheetId);
                DanceMarkingSheetDto deletedMarkedSheetDto = _mapper.Map<DanceMarkingSheetDto>(deletedMarkedSheet);
                return Ok(deletedMarkedSheetDto);
            }


            return NotFound();
        }

        [HttpGet]
        [Route("{danceId}/markingsheet")]
        public async Task<IActionResult> GetMarkingSheets([FromRoute] int danceId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var danceMarkingSheets = await _danceService.ListMarkingSheets(danceId);

            List<DanceMarkingSheetDto> dancemarkingSheetDtos = _mapper.Map<List<DanceMarkingSheetDto>>(danceMarkingSheets);

            return Ok(dancemarkingSheetDtos);
        }

        [HttpGet]
        [Route("{danceId}/markingsheet/{markingSheetId}")]
        public async Task<IActionResult> GetMarkingSheet([FromRoute] int danceId, [FromRoute] int markingSheetId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var danceMarkingSheet = await _danceService.GetMarkingSheet(markingSheetId);

            return Ok(danceMarkingSheet);
        }

        [HttpGet]
        [Route("{danceId}/judgeslotinfo")]
        public async Task<IActionResult> GetJudgeSlotInformation([FromRoute] int danceId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var judgeSlotInformationDos = await _judgeSlotService.GetSlotsByDanceId(danceId);

            List<JudgeSlotInformationDto> judgeSlotInfoDtos = _mapper.Map<List<JudgeSlotInformationDto>>(judgeSlotInformationDos);

            return Ok(judgeSlotInfoDtos);
        }

        [HttpPut]
        [Route("{danceId}/judgeslotinfo")]
        public async Task<IActionResult> UpdateJudgeSlotInformation([FromRoute] int danceId, [FromBody] List<JudgeSlotInformationDto> updateJudgeSlotInformation)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            await _judgeSlotService.UpdateJudgeSlotInformationForDance(danceId, updateJudgeSlotInformation);

            // Reload the data after the updates
            var judgeSlotInformationDos = await _judgeSlotService.GetSlotsByDanceId(danceId);

            List<JudgeSlotInformationDto> judgeSlotInfoDtos = _mapper.Map<List<JudgeSlotInformationDto>>(judgeSlotInformationDos);

            return Ok(judgeSlotInfoDtos);
        }

        [HttpPut("{id}/hasscoreschecked")]
        public async Task<IActionResult> UpdateCheckedState(int id, [FromBody] DanceCheckedUpdateSubmissionDto updateCheckedStateSubmission)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            await _danceService.UpdateCheckedState(id, updateCheckedStateSubmission.HasScoresChecked);

            return NoContent();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private async Task<DanceDetailDto> MappingSupport_TeamImage(Dance dance, DanceDetailDto danceDetailDto)
        {
            try
            {
                //Get the team image ()
                var teamImages = dance.TeamAttendance.Team.TeamImages;

                TeamImage primaryTeamImage = null;
                if (teamImages != null && teamImages.Count() > 0)
                {
                    //Use the primary or the last(latest)
                    primaryTeamImage = teamImages.First(ti => ti.IsPrimary) ?? teamImages.Last();
                }

                if (primaryTeamImage != null)
                {
                    var teamImage = await _imageService.FindById(primaryTeamImage.ImageId);

                    //set to migrated image
                    danceDetailDto.TeamPictureUrl = teamImage.ImageUri;
                }
                else
                {
                    danceDetailDto.TeamPictureUrl = string.Empty;
                }  
            }
            catch
            {
                danceDetailDto.TeamPictureUrl = string.Empty;
            }

            return danceDetailDto;
        }
    }
}
