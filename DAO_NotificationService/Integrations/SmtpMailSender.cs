using Helpers.Models.NotificationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DAO_NotificationService.Integrations
{
    public static class SmtpMailSender
    {
        public static string SendEmail(SendEmailModel model)
        {
            try
            {
                List<MailAddress> lst = new List<MailAddress>();

                System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage();
                m.From = new System.Net.Mail.MailAddress(Program._settings.EmailAddress, Program._settings.EmailDisplayName);

                foreach (var item in model.To)
                {
                    m.To.Add(new MailAddress(item));
                }
                foreach (var item in model.Cc)
                {
                    m.CC.Add(new MailAddress(item));
                }
                foreach (var item in model.Bcc)
                {
                    m.Bcc.Add(new MailAddress(item));
                }

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(Program._settings.EmailHost);

                m.Subject = model.Subject;
                m.Body = WrapToMailTemplate(model.Content);
                m.IsBodyHtml = true;

                smtp.Port = Convert.ToInt32(Program._settings.EmailPort);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (Convert.ToBoolean(Program._settings.EmailSSL))
                    smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(Program._settings.EmailAddress, Program._settings.EmailPassword);
                smtp.Send(m);

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string WrapToMailTemplate(string content)
        {
            string all =
            "<style>.btnspecial{text-decoration:none;padding:10px 20px;border-radius:5px;background:#334d80;color:#f3f3f3;}</style>" +

            "<div style=\"width:100%;height:100px;background:#353847 !important;border-bottom: 5px solid #334d80;\">" +
                "<center>" +
                    "<img style=\"margin-top:30px;width: 160px;\" src=\"" + Program._settings.WebPortal_Url + "/Public/images/" + Program._settings.DAOName + "-Logo-Mail.png\">" +
                "</center>" +
            "</div>" +

            "<div style=\"width:100%;min-height:300px;background:#f3f3f3 !important;padding:50px;color:#25262f;font-family:Arial\">" +
                         content +
            "</div>" +

            "<div style=\"width:100%;height:100px;background:#353847 !important;border-top: 5px solid #334d80;\">" +
                "<center>" +
                    "<br><br>" +
                    "<a style=\"color:#f3f3f3 !important\" href=\"\">Unsubscribe</a>" +
                "</center>" +
            "</div>";

            return all;
        }
    }
}
