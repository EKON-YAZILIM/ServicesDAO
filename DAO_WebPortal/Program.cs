using Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_WebPortal
{
    public class Program
    {
        public class Settings
        {
            public string RabbitMQUrl { get; set; }
            public string RabbitMQUsername { get; set; }
            public string RabbitMQPassword { get; set; }
            public string Service_ApiGateway_Url { get; set; }
            public string EncryptionKey { get; set; }

            //DAO PARAMETERS
            public List<string> DosCurrencies { get; set; }
            public List<double> DosFees { get; set; }
            public double DefaultPolicingRate { get; set; }
            public double MinPolicingRate { get; set; }
            public double MaxPolicingRate { get; set; }
            public bool ForumKYCRequired { get; set; }
            public double QuorumRatio { get; set; }
            public int InternalAuctionDays { get; set; }
            public int PublicAuctionDays { get; set; }
            public int InformalVotingDays { get; set; }
            public int FormalVotingDays { get; set; }
            public double ReputationConversionRate { get; set; }
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
