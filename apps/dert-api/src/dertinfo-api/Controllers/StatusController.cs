using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DertInfo.CrossCutting.Connection;

namespace DertInfo.Api.Controllers
{
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        public IStorageAccountConnection _stoageAccountConfiguration; 

        public StatusController(IStorageAccountConnection stoageAccountConfiguration) {

            _stoageAccountConfiguration = stoageAccountConfiguration;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { _stoageAccountConfiguration.getScoreSheetsContainer(), "value1", "value2" };
        }
    }
}
