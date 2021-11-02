using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;

namespace DAO_WebPortal.Utility
{
    /// <summary>
    ///  IpHelper class for extracting client ip and port from HttpContext object.
    /// </summary>
    public static class IpHelper
    {
        /// <summary>
        ///  Get client IP address from HttpContext object
        /// </summary>
        /// <param name="context">HttpContext object</param>
        /// <returns>Client IP Address</returns>
        public static string GetClientIpAddress(Microsoft.AspNetCore.Http.HttpContext context)
        {

            string ip = "";

            try
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return ip;
        }

        /// <summary>
        ///  Get client port from HttpContext object
        /// </summary>
        /// <param name="context">HttpContext object</param>
        /// <returns>Client port</returns>
        public static string GetClientPort(Microsoft.AspNetCore.Http.HttpContext context)
        {
            string port = "";

            try
            {
                port = context.Connection.RemotePort.ToString();
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return port;
        }
    }
}
