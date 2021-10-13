using Helpers;
using Helpers.Constants;
using Helpers.Models.DtoModels.LogDbDto;
using Helpers.Models.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;

namespace DAO_LogService
{
    public class MainEvents
    {
        public static void IntiliazeLogService()
        {
            try
            {
                TimerEvents.StartTimers();

                Program.rabbitMq.ExchangeDeclare(FeedNames.ApplicationLogs, "direct", false, false);
                Program.rabbitMq.ExchangeDeclare(FeedNames.ErrorLogs, "direct", false, false);
                Program.rabbitMq.ExchangeDeclare(FeedNames.UserLogs, "direct", false, false);

                Program.rabbitMq.QueueBind(FeedNames.ApplicationLogs, "", OnApplicationLog);
                Program.rabbitMq.QueueBind(FeedNames.ErrorLogs, "", OnErrorLog);
                Program.rabbitMq.QueueBind(FeedNames.UserLogs, "", OnUserLog);
            }
            catch (Exception ex)
            {
                Program.monitizer.startSuccesful = -1;
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
        }

        public static void OnApplicationLog(string source, byte[] data)
        {
            ApplicationLogDto log = Serializers.Deserialize<ApplicationLogDto>(data);
            Program.ApplicationLogs.Enqueue(log);

            Program.monitizer.AddConsole(source + " OnApplicationLog :: " + log.Explanation);
        }

        public static void OnErrorLog(string source, byte[] data)
        {
            ErrorLogDto log = Serializers.Deserialize<ErrorLogDto>(data);
            Program.ErrorLogs.Enqueue(log);

            Program.monitizer.AddConsole(source + " OnErrorLog :: " + log.Message);
        }

        public static void OnUserLog(string source, byte[] data)
        {
            UserLogDto log = Serializers.Deserialize<UserLogDto>(data);
            Program.UserLogs.Enqueue(log);

            Program.monitizer.AddConsole(source + " OnUserLog :: " + log.Explanation);
        }

    }
}
