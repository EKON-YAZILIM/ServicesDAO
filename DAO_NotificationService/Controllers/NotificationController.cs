using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Helpers.Constants.Enums;

namespace DAO_NotificationService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : Controller
    {
        [Route("SendEmail")]
        [HttpPost]
        public string SendEmail(SendEmailModel model)
        {
            try
            {
                if(Program._settings.EmailChannel.ToLower() == "smtp")
                {
                    string res = DAO_NotificationService.Integrations.SmtpMailSender.SendEmail(model);
                    Program.monitizer.AddConsole("Email sent. To:" + model.To + " Channel: SMTP");
                    return "Success";

                }
                if (Program._settings.EmailChannel.ToLower() == "api")
                {
                    string res = DAO_NotificationService.Integrations.ApiMailSender.SendEmail(model);
                    Program.monitizer.AddConsole("Email sent. To:" + model.To + " Channel: API");
                    return "Success";
                }

                return "Channel not found";
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return "Error";
            }
        }

    }
}