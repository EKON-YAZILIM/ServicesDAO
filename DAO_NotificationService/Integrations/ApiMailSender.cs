using Helpers.Models.NotificationModels;
using Helpers.Models.SharedModels;
using System;

namespace DAO_NotificationService.Integrations
{
    public class ApiMailSender
    {
        public static string SendEmail(SendEmailModel model)
        {
            try
            {
                string jsonResult = Helpers.Request.Get(Program._settings.EmailApiUrl + "?email=" + string.Join(',', model.To) + "&title=" + model.Subject + "&explanation=" + model.Content);

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
