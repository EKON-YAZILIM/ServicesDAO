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
            //Folder containing the views. Different views can be shown for different organizations. For example for CRDAO Views-CRDAO folder can be used.
            //Default folder containing ServicesDAO content is "Views"
            public string ViewFolder { get; set; }

            //NETWORK VARIABLES
            public string RabbitMQUrl { get; set; }
            public string RabbitMQUsername { get; set; }
            public string RabbitMQPassword { get; set; }
            public string Service_ApiGateway_Url { get; set; }
   

            public string EncryptionKey { get; set; }

            //DAO VARIABLES

            //Dos fee currencies
            public List<string> DosCurrencies { get; set; }

            //Dos fee list (The amount that people pay to be post a job)
            public List<double> DosFees { get; set; }

            //Default policing rate (What % goes to the DAO Members vs what % goes to the OP)
            public double DefaultPolicingRate { get; set; }

            //Min policing rate (What is the minimum policing rate that a user can set on a job’s price - non-retroactive)
            public double MinPolicingRate { get; set; }

            //Max policing rate (What is the maximum policing rate that a user can set on a job’s price - non-retroactive)
            public double MaxPolicingRate { get; set; }

            //KYC for normal forum user (Do forum users need to do KYC)
            public bool ForumKYCRequired { get; set; }

            //Quorum ratio (% of quorum needed to pass informal and formal votes)
            public double QuorumRatio { get; set; }

            //Timeframe for internal auction
            public int InternalAuctionTime { get; set; }

            //Timeframe for external auction
            public int PublicAuctionTime { get; set; }

            //Time type for auction (Weeks, days, minutes)
            public string AuctionTimeType { get; set; }

            //Timeframe for votings
            public int VotingTime { get; set; }

            //Time type for votings (Weeks, days, minutes)
            public string VotingTimeType { get; set; }

            //Cost -> Reputation conversion rate (Specifies the conversion rate from a job’s bid price amount when minting reputation for a proposal to the system)
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
