using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;

namespace DAO_WebPortal.Utility
{
    public static class Methods
    {
        public static string GetClientIpAddress(Microsoft.AspNetCore.Http.HttpContext request)
        {

            string ip = "";

            try
            {
                ip = request.Connection.RemoteIpAddress.ToString();
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return ip;
        }

        public static string GetClientPort(Microsoft.AspNetCore.Http.HttpContext request)
        {
            string port = "";

            try
            {
                port = request.Connection.RemotePort.ToString();
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return port;
        }
    }
}
