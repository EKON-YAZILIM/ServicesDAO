using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.SharedModels
{
    /// <summary>
    ///  Microservice heartbeat status class
    /// </summary>
    [Serializable]
    public class AppStatus
    {
        /// <summary>
        ///  Name of the application
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        ///  Date of app status message
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        ///  Ip of the application
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        ///  Number of exceptions occured in the application.
        /// </summary>
        public int ErrorCount { get; set; }
        /// <summary>
        ///  Number of fatal exceptions occured in the application. (Fatal exception: Exceptions which prevents service to work properly.)
        /// </summary>
        public int FatalCount { get; set; }
        /// <summary>
        ///  Date of app status message
        ///  Service start status
        ///  <value>-1: Application has fatal errors</value>
        ///  <value>0: Application has errors</value>
        ///  <value>1: Application healthy</value>
        /// </summary>
        public int Status { get; set; }
    }
}
