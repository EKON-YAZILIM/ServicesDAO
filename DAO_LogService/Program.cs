using Helpers;
using Helpers.Models.DtoModels.LogDbDto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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

        public static Monitizer monitizer;
        public static Settings _settings { get; set; } = new Settings();
        public static Helpers.RabbitMQ rabbitMq = new Helpers.RabbitMQ();
        public static Mysql mysql = new Helpers.Mysql();
        public static DbContextOptions dbOptions;

        public static ConcurrentQueue<UserLogDto> UserLogs = new ConcurrentQueue<UserLogDto>();
        public static ConcurrentQueue<ApplicationLogDto> ApplicationLogs = new ConcurrentQueue<ApplicationLogDto>();
        public static ConcurrentQueue<ErrorLogDto> ErrorLogs = new ConcurrentQueue<ErrorLogDto>();

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
