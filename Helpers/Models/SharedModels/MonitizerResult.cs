using Helpers.Models.DtoModels.LogDbDto;
using System.Collections.Generic;

namespace Helpers.Models.SharedModels
{
    /// <summary>
    ///  Microservice info and health check class
    /// </summary>
    public class MonitizerResult
    {
        /// <summary>
        ///  Number of fatal exceptions occured in the application. (Fatal exception: Exceptions which prevents service to work properly.)
        /// </summary>
        public int FatalCounter { get; set; }

        /// <summary>
        ///  Number of exceptions occured in the application.
        /// </summary>
        public int ExceptionCounter { get; set; }

        /// <summary>
        ///  Service start status
        ///  <value>-1: Application error in startup</value>
        ///  <value>0: Application loading</value>
        ///  <value>1: Application started successfully</value>
        /// </summary>
        public int StartSuccesful { get; set; }

        /// <summary>
        ///  List of exceptions occured in the application
        /// </summary>
        public List<ErrorLogDto> Exceptions { get; set; }

        /// <summary>
        ///  List of application type logs in the application
        /// </summary>
        public List<ApplicationLogDto> Logs { get; set; }

        /// <summary>
        ///  Application name
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        ///  Application hosting ip
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        ///  Fast logs for diagnose on the application start page. (Not stored in DB, similar to Console.WriteLine())
        /// </summary>
        public List<string> Console { get; set; }
    }
}