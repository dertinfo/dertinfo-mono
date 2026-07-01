using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Repository;
using DertInfo.Services;
using DertInfo.Services.Entity.Dances;
using DertInfo.Services.Entity.Images;
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
    public class VenueController : AuthController
    {
        IMapper _mapper;
        IVenueService _venueService;
        IDanceService _danceService;
        IImageService _imageService;

        public VenueController(
            IMapper mapper,
            IVenueService venueService,
            IDanceService danceService,
            IImageService imageService,
            IDertInfoUser user
            ) : base(user)
        {

            _mapper = mapper;
            _venueService = venueService;
            _danceService = danceService;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var venues = await _venueService.ListByUser();

            List<VenueDto> venueDtos = _mapper.Map<List<VenueDto>>(venues);

            return Ok(venueDtos.OrderBy(dto => dto.Name));
        }

        [Authorize]
        [HttpGet("{id}/dances")]
        public async Task<IActionResult> GetDances(int id)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var dances = await _danceService.ListByVenueId(id);

            List<VenueDanceDto> danceDtos = _mapper.Map<List<VenueDanceDto>>(dances);

            //Fill out the image
            foreach (var danceDto in danceDtos)
            {
                try
                {
                    var matchingDance = dances.First(d => d.Id == danceDto.DanceId);

                    //Try to find the primary image
                    var teamImages = matchingDance.TeamAttendance.Team.TeamImages;

                    //If the is no primary check for others
                    TeamImage primaryTeamImage = null;
                    if (teamImages.Count() > 0)
                    {
                        primaryTeamImage = teamImages.First(ti => ti.IsPrimary) ?? teamImages.First();
                        var image = await _imageService.FindById(primaryTeamImage.ImageId);

                        if (image != null)
                        {
                            danceDto.TeamPictureUrl = image.ImageUri;
                        }
                        else
                        {
                            danceDto.TeamPictureUrl = "";
                        }
                    }
                    else
                    {
                        danceDto.TeamPictureUrl = "";
                    }
                }
                catch
                {
                    danceDto.TeamPictureUrl = "";
                }
            }

            return Ok(danceDtos.OrderBy(dto => dto.TeamName));
        }
    }
}
