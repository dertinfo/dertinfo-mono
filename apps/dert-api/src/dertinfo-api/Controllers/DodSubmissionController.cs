using AutoMapper;
using DertInfo.Models.DataTransferObject.DertOfDerts;
using DertInfo.Services.Entity.DodSubmissions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Controllers
{
    /// <summary>
    /// A public facing controller that allows retrival of information for the Dert of Derts submissions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DodSubmissionController : Controller
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IDodSubmissionService _dodSubmissionService;

        public DodSubmissionController(
            IMapper mapper,
            IDodSubmissionService dodSubmissionService
            )
        {
            _mapper = mapper;
            _dodSubmissionService = dodSubmissionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dodSubmissions = await _dodSubmissionService.ListAll();

            List<DodSubmissionDto> dodSubmissionDtos = _mapper.Map<List<DodSubmissionDto>>(dodSubmissions);

            return Ok(dodSubmissionDtos.OrderBy(dto => dto.GroupName));
        }

    }
}
