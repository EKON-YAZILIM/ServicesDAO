using Helpers.Constants;
using Helpers.Models.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;

namespace DAO_NotificationService
{
    public class MainEvents
    {
        public static void InitializeNotificationService()
        {
            try
            {
                Program.rabbitMq.ExchangeDeclare(FeedNames.NotificationFeed, "topic", false, false);
                Program.rabbitMq.QueueBind(FeedNames.NotificationFeed, "*", OnEmailMessageReceived);
            }
            catch (Exception ex)
            {
                Program.monitizer.startSuccesful = -1;
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
        }

        public static void OnEmailMessageReceived(string application, byte[] data)
        {
            try
            {
                SendEmailModel notif = Helpers.Serializers.Deserialize<SendEmailModel>(data);

                Controllers.NotificationController emailSender = new Controllers.NotificationController();
                emailSender.SendEmail(notif);

                Program.monitizer.AddConsole("Notification received : " + Helpers.Serializers.SerializeJson(notif));
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
            }
        }


    }
}
