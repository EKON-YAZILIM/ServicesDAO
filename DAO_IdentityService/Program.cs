using Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_IdentityService
{
    public class Program
    {
        public class Settings
        {
            public string DAOName { get; set; }

            public string DbConnectionString { get; set; }

            public string RabbitMQUrl { get; set; }
            public string RabbitMQUsername { get; set; }
            public string RabbitMQPassword { get; set; }

            public string Service_Db_Url { get; set; }
            public string WebPortal_Url { get; set; }
            public string Service_Notification_Url { get; set; }

            public string EncryptionKey { get; set; }
            public string JwtTokenKey { get; set; }
            public string KYCID { get; set; }
            public string KYCURL { get; set; }
            public string KYCFORMID { get; set; }
            public string WebURLforKYCResponse { get; set; }

        }

        public static Settings _settings { get; set; } = new Settings();
        public static Helpers.RabbitMQ rabbitMq = new Helpers.RabbitMQ();
        public static Monitizer monitizer;

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
