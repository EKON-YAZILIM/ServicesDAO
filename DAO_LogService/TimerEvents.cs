using DAO_LogService.Models;
using Helpers.Models.DtoModels.LogDbDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using static Helpers.Constants.Enums;

namespace DAO_LogService
{
    public class TimerEvents
    {
        public static System.Timers.Timer LogDbTimer = new System.Timers.Timer();

        public static bool saveDbProgress = false;

        public static void StartTimers()
        {
            LogDbTimer.Elapsed += new ElapsedEventHandler(SaveToDb);
            LogDbTimer.Interval = 10000;
            LogDbTimer.Enabled = true;
        }

        private static void SaveToDb(object source, ElapsedEventArgs e)
        {
            if (!saveDbProgress)
            {
                saveDbProgress = true;

                try
                {
                    List<ApplicationLogDto> appLogList = new List<ApplicationLogDto>();
                    List<UserLogDto> userLogList = new List<UserLogDto>();
                    List<ErrorLogDto> errorLogList = new List<ErrorLogDto>();

                    while (Program.ApplicationLogs.Count > 0)
                    {
                        ApplicationLogDto log;
                        Program.ApplicationLogs.TryDequeue(out log);
                        if (log != null)
                            appLogList.Add(log);
                    }

                    while (Program.UserLogs.Count > 0)
                    {
                        UserLogDto log;
                        Program.UserLogs.TryDequeue(out log);
                        if (log != null)
                            userLogList.Add(log);
                    }

                    while (Program.ErrorLogs.Count > 0)
                    {
                        ErrorLogDto log;
                        Program.ErrorLogs.TryDequeue(out log);
                        if (log != null)
                            errorLogList.Add(log);
                    }

                    using (Contexts.dao_logsdb_context db = new Contexts.dao_logsdb_context())
                    {

                        if (appLogList.Count > 0)
                        {
                            List<ApplicationLog> items = Mapping.AutoMapperBase._mapper.Map<List<ApplicationLogDto>, List<ApplicationLog>>(appLogList);
                            db.ApplicationLogs.AddRange(items);
                            db.SaveChanges();
                        }

                        if (userLogList.Count > 0)
                        {
                            List<UserLog> items = Mapping.AutoMapperBase._mapper.Map<List<UserLogDto>, List<UserLog>>(userLogList);
                            db.UserLogs.AddRange(items);
                            db.SaveChanges();
                        }

                        if (errorLogList.Count > 0)
                        {
                            List<ErrorLog> items = Mapping.AutoMapperBase._mapper.Map<List<ErrorLogDto>, List<ErrorLog>>(errorLogList);
                            db.ErrorLogs.AddRange(items);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                }
            }

            saveDbProgress = false;
        }
    }
}
