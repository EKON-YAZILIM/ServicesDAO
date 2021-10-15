using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Helpers.Models.NotificationModels;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Mvc;
using static Helpers.Constants.Enums;

namespace DAO_NotificationService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : Controller
    {
        /// <summary>
        ///  Send notification email method
        /// </summary>
        /// <param name="model">SendEmailModel</param>
        /// <returns>Is successful</returns>
        [Route("SendEmail")]
        [HttpPost]
        public AjaxResponse SendEmail(SendEmailModel model)
        {
            try
            {
                //Send with SMTP channel
                if(Program._settings.EmailChannel.ToLower() == "smtp")
                {
                    string res = DAO_NotificationService.Integrations.SmtpMailSender.SendEmail(model);
                    Program.monitizer.AddConsole("Email sent. To:" + Serializers.SerializeJson(model.To) + " Channel: SMTP");
                    return new AjaxResponse() { Success = true, Message = "Success" };

                }
                //Send with API channel
                if (Program._settings.EmailChannel.ToLower() == "api")
                {
                    string res = DAO_NotificationService.Integrations.ApiMailSender.SendEmail(model);
                    Program.monitizer.AddConsole("Email sent. To:" + Serializers.SerializeJson(model.To) + " Channel: API");
                    return new AjaxResponse() { Success = true, Message = "Success" };
                }

                return new AjaxResponse() { Success = false, Message = "Channel not found" };
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AjaxResponse() { Success = false, Message = ex.Message };
            }
        }

    }
}