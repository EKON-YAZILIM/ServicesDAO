using Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_LogService
{
    public class Program
    {
        public class Settings
        {
            public string DbConnectionString { get; set; }

            public string RabbitMQUrl { get; set; }
            public string RabbitMQUsername { get; set; }
            public string RabbitMQPassword { get; set; }
        }

        public static Settings _settings { get; set; } = new Settings();
        public static Helpers.RabbitMQ rabbitMq = new Helpers.RabbitMQ();
        public static Monitizer monitizer;
        public static Mysql mysql = new Helpers.Mysql();

        //public static ConcurrentQueue<UserLogsDto> UserLogs = new ConcurrentQueue<UserLogsDto>();
        //public static ConcurrentQueue<ApplicationLogsDto> ApplicationLogs = new ConcurrentQueue<ApplicationLogsDto>();
        //public static ConcurrentQueue<ErrorLogsDto> ErrorLogs = new ConcurrentQueue<ErrorLogsDto>();

        //public static Dictionary<string, List<ErrorLogsDto>> errorLogController = new Dictionary<string, List<ErrorLogsDto>>();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
