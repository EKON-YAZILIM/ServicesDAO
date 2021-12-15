using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Helpers.Models.DtoModels.MainDbDto;
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
        public SimpleResponse SendEmail(SendEmailModel model)
        {
            try
            {
                if (model.TargetGroup != null)
                {

                }

                //Send with SMTP channel
                if (Program._settings.EmailChannel.ToLower() == "smtp")
                {
                    string res = DAO_NotificationService.Integrations.SmtpMailSender.SendEmail(model);
                    Program.monitizer.AddConsole("Email sent. To:" + Serializers.SerializeJson(model.To) + " Channel: SMTP");
                    return new SimpleResponse() { Success = true, Message = "Success" };
                }

                return new SimpleResponse() { Success = false, Message = "Channel not found" };
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new SimpleResponse() { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        ///  Send notification email to system admin method
        /// </summary>
        /// <param name="model">SendEmailModel (To, Cc, Bcc fields are ignored in this request)</param>
        /// <returns>Is successful</returns>
        [Route("SendPublicContactEmail")]
        [HttpPost]
        public SimpleResponse SendPublicContactEmail(SendEmailModel model)
        {
            try
            {
                //Get emails of Admins
                string userJson = Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetAdminUsers");
                //Parse response
                var usersObj = Helpers.Serializers.DeserializeJson<List<UserDto>>(userJson);

                //Send email to first Admin and pre defined contact email
                if (usersObj.Count > 0)
                {
                    model.To = new List<string>() { usersObj[0].Email, Program._settings.ContactEmail };
                }

                //Send with SMTP channel
                if (Program._settings.EmailChannel.ToLower() == "smtp")
                {
                    string res = DAO_NotificationService.Integrations.SmtpMailSender.SendEmail(model);
                    Program.monitizer.AddConsole("Email sent. To:" + Serializers.SerializeJson(model.To) + " Channel: SMTP");
                    return new SimpleResponse() { Success = true, Message = "Success" };
                }

                return new SimpleResponse() { Success = false, Message = "Channel not found" };
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new SimpleResponse() { Success = false, Message = ex.Message };
            }
        }
    }
}