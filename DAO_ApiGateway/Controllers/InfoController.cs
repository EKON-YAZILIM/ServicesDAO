using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_ApiGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet("GetAppInfo", Name = "GetAppInfo")]
        public MonitizerResult GetAppInfo()
        {
            return Program.monitizer.GetMonitizerResult();
        }

        [HttpGet("ResetErrors", Name = "ResetErrors")]
        public bool ResetErrors()
        {
            Program.monitizer.exceptions.Clear();
            Program.monitizer.exceptionCounter = 0;
            Program.monitizer.fatalCounter = 0;
            return true;
        }
    }
}
