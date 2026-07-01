using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Services.Entity.DodTalks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DodTalkController : Controller
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IDodTalkService _dodTalkService;

        public DodTalkController(
            IMapper mapper,
            IDodTalkService dodTalkService
            )
        {
            _mapper = mapper;
            _dodTalkService = dodTalkService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dodTalks = await _dodTalkService.ListAll();

            // Perform Auto Map of simple fields
            List<DodTalkDto> dodTalkDtos = _mapper.Map<List<DodTalkDto>>(dodTalks);

            return Ok(dodTalkDtos.OrderBy(dto => dto.BroadcastDateTime));
        }
    }
}
