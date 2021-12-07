using Helpers;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static DAO_WebPortal.Program;
using static Helpers.Constants.Enums;

namespace DAO_WebPortal
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            //Get related appsettings.json file (Development or Production)
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            LoadConfig(configuration);
            InitializeService();
        }

        /// <summary>
        ///  Loads application config from appsettings.json
        /// </summary>
        /// <param name="configuration"></param>
        public static void LoadConfig(IConfiguration configuration)
        {
            var config = configuration.GetSection("PlatformSettings");
            config.Bind(_settings);

            LoadDaoSettings();
        }


        /// <summary>
        ///  Initializes application (Db migrations, connection check, timer construction)
        /// </summary>
        public static void InitializeService()
        {
            Helpers.Encryption.EncryptionKey = Program._settings.EncryptionKey;

            monitizer = new Monitizer(_settings.RabbitMQUrl, _settings.RabbitMQUsername, _settings.RabbitMQPassword);

            ApplicationStartResult rabbitControl = rabbitMq.Initialize(_settings.RabbitMQUrl, _settings.RabbitMQUsername, _settings.RabbitMQPassword);
            if (!rabbitControl.Success)
            {
                monitizer.startSuccesful = -1;
                monitizer.AddException(rabbitControl.Exception, LogTypes.ApplicationError, true);
            }

            if (monitizer.startSuccesful != -1)
            {
                monitizer.startSuccesful = 1;
                monitizer.AddApplicationLog(LogTypes.ApplicationLog, monitizer.appName + " application started successfully.");
            }
        }

        /// <summary>
        ///  Loads platform settings from db if exists.
        /// </summary>
        public static void LoadDaoSettings()
        {
            //Get latest platform settings (DAO variables from db)
            string settingsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/PublicActions/GetLatestSetting");
            //If custom settings found
            if (!string.IsNullOrEmpty(settingsJson))
            {
                PlatformSettingDto settings = Serializers.DeserializeJson<PlatformSettingDto>(settingsJson);

                if (!string.IsNullOrEmpty(settings.DosCurrencies))
                {
                    Program._settings.DosCurrencies = settings.DosCurrencies.Split(',').ToList();
                }

                if (!string.IsNullOrEmpty(settings.DosFees))
                {
                    Program._settings.DosFees = settings.DosFees.Split(',').ToList().Select(x => double.Parse(x)).ToList();
                }

                if (settings.DefaultPolicingRate != null)
                {
                    Program._settings.DefaultPolicingRate = Convert.ToDouble(settings.DefaultPolicingRate);
                }

                if (settings.MinPolicingRate != null)
                {
                    Program._settings.MinPolicingRate = Convert.ToDouble(settings.MinPolicingRate);
                }

                if (settings.MaxPolicingRate != null)
                {
                    Program._settings.MaxPolicingRate = Convert.ToDouble(settings.MaxPolicingRate);
                }

                Program._settings.ForumKYCRequired = settings.ForumKYCRequired;

                if (settings.QuorumRatio != null)
                {
                    Program._settings.QuorumRatio = Convert.ToDouble(settings.QuorumRatio);
                }

                if (settings.InternalAuctionTime != null)
                {
                    Program._settings.InternalAuctionTime = Convert.ToInt32(settings.InternalAuctionTime);
                }

                if (settings.PublicAuctionTime != null)
                {
                    Program._settings.PublicAuctionTime = Convert.ToInt32(settings.PublicAuctionTime);
                }

                if (!string.IsNullOrEmpty(settings.AuctionTimeType))
                {
                    Program._settings.AuctionTimeType = settings.AuctionTimeType;
                }

                if (settings.VotingTime != null)
                {
                    Program._settings.VotingTime = Convert.ToInt32(settings.VotingTime);
                }

                if (!string.IsNullOrEmpty(settings.VotingTimeType))
                {
                    Program._settings.VotingTimeType = settings.VotingTimeType;
                }

                if (settings.ReputationConversionRate != null)
                {
                    Program._settings.ReputationConversionRate = Convert.ToDouble(settings.ReputationConversionRate);
                }
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Changes default Views folder
            if (!string.IsNullOrEmpty(Program._settings.ViewFolder))
            {
                services.Configure<RazorViewEngineOptions>(o =>
                {
                    o.ViewLocationFormats.Clear();
                    o.ViewLocationFormats.Add("/" + Program._settings.ViewFolder + "/{1}/{0}" + RazorViewEngine.ViewExtension);
                    o.ViewLocationFormats.Add("/" + Program._settings.ViewFolder + "/Shared/{0}" + RazorViewEngine.ViewExtension);
                });
            }

            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddControllersWithViews();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddLocalization(o => o.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new[] { new CultureInfo("en-GB") };
                opts.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("tr-TR");
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;

            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //services.AddHsts(options =>
            //{
            //    //options.MaxAge = TimeSpan.FromDays(100);
            //    options.IncludeSubDomains = true;
            //    options.Preload = true;
            //});

            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Error");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseStaticFiles();

            //app.UseHttpsRedirection();

            var defaultDateCulture = "en-US";
            var ci = new CultureInfo(defaultDateCulture);
            ci.NumberFormat.NumberDecimalSeparator = ".";
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            ci.NumberFormat.NumberGroupSeparator = ",";

            // Configure the Localization middleware
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<CultureInfo> { ci, },
                SupportedUICultures = new List<CultureInfo> { ci, }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Public}/{action=Index}/{id?}");
            });
        }
    }
}
