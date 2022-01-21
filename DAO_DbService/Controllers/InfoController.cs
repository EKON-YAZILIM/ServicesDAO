using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Controllers
{
    /// <summary>
    ///  InfoController contains healthcheck methods for the application
    ///  This controller only responds requests from local machine (LocalMachineAuthorization)
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        /// <summary>
        ///  Get application information, logs, errors etc. 
        /// </summary>
        /// <returns>MonitizerResult class</returns>
        [HttpGet("GetAppInfo", Name = "GetAppInfo")]
        public MonitizerResult GetAppInfo()
        {
            return Program.monitizer.GetMonitizerResult();
        }

        /// <summary>
        ///  Reset application exception list.
        /// </summary>
        /// <returns></returns>
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
