using Helpers.Constants;
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
            //ApplicationLogsDto log = Serializers.Deserialize<ApplicationLogsDto>(data);
            //Program.ApplicationLogs.Enqueue(log);

            //Program.monitizer.AddConsole(source + " " + log.Explanation);

            //Program.monitizer.AddConsole(source + " OnApplicationLog :: " + log.Explanation);
        }

        public static void OnErrorLog(string source, byte[] data)
        {
            //ErrorLogsDto log = Serializers.Deserialize<ErrorLogsDto>(data);

            //if (!Program.errorLogController.ContainsKey(log.Application))
            //{
            //    Program.errorLogController.Add(log.Application, new System.Collections.Generic.List<ErrorLogsDto>());
            //}

            //var controlList = Program.errorLogController[log.Application];

            //if (controlList.Count(x => x.Date > DateTime.Now.AddMinutes(-5)) > 50)
            //{
            //    if (Program.errorLogController[log.Application].Count(x => x.Message == "Max log control reached.") == 0)
            //    {
            //        log = new ErrorLogsDto() { Application = log.Application, Date = DateTime.Now, Message = "Max log control reached.", Type = "Log" };
            //        Program.errorLogController[log.Application].Add(log);
            //        Program.ErrorLogs.Enqueue(log);
            //    }
            //}
            //else
            //{
            //    Program.ErrorLogs.Enqueue(log);
            //    Program.errorLogController[log.Application].Add(log);
            //}

            //if (controlList.Count > 100)
            //{
            //    Program.errorLogController[log.Application].RemoveAt(0);
            //}

            //Program.monitizer.AddConsole(source + " OnErrorLog :: " + log.Message);
        }

        public static void OnUserLog(string source, byte[] data)
        {
            //UserLogsDto log = Serializers.Deserialize<UserLogsDto>(data);
            //Program.UserLogs.Enqueue(log);

            //Program.monitizer.AddConsole(source + " OnUserLog :: " + log.Explanation);
        }

    }
}
