using AutoMapper;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Services;
using DertInfo.Services.Entity.Events;
using DertInfo.Services.Entity.Images;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    [Route("api/[controller]")]
    public class ShowcaseController : Controller
    {
        IMapper _mapper;
        IEventService _eventsService;
        ITeamService _teamService;
        IImageService _imageService;

        public ShowcaseController(
            IMapper mapper,
            IEventService eventsService,
            IImageService imageService,
            ITeamService teamService
            )
        {
            _mapper = mapper;
            _eventsService = eventsService;
            _imageService = imageService;
            _teamService = teamService;
        }

        [HttpGet]
        [Route("teams")]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _teamService.GetAllForShowcase();

            // Perform Auto Map of simple fields
            List<TeamShowcaseDto> teamShowcaseDtos = _mapper.Map<List<TeamShowcaseDto>>(teams);

            return Ok(teamShowcaseDtos.OrderBy(dto => dto.TeamName));
        }

        [HttpGet]
        [Route("events")]
        public async Task<IActionResult> GetEvents()
        {
            var myEvents = await _eventsService.ListForShowcase();

            // Perform Auto Map of simple fields
            List<EventShowcaseDto> eventShowcaseDtos = _mapper.Map<List<EventShowcaseDto>>(myEvents);

            return Ok(eventShowcaseDtos.OrderByDescending(dto => dto.EventStartDate));
        }

        [HttpGet]
        [Route("event/{eventId}")]
        public async Task<IActionResult> GetEvents([FromRoute]int eventId)
        {
            var myEvent = await _eventsService.DetailForShowcase(eventId);

            // Perform Auto Map of simple fields
            EventShowcaseDetailDto eventShowcaseDto = _mapper.Map<EventShowcaseDetailDto>(myEvent);

            return Ok(eventShowcaseDto);
        }
    }
}
