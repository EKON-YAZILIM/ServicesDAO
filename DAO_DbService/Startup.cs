using Helpers;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_DbService.Program;
using Microsoft.EntityFrameworkCore;
using DAO_DbService.Contexts;
using System.Threading;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace DAO_DbService
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
        }

        /// <summary>
        ///  Initializes application (Db migrations, connection check, timer construction)
        /// </summary>
        public static void InitializeService()
        {
            monitizer = new Monitizer(_settings.RabbitMQUrl, _settings.RabbitMQUsername, _settings.RabbitMQPassword);

            ApplicationStartResult rabbitControl = rabbitMq.Initialize(_settings.RabbitMQUrl, _settings.RabbitMQUsername, _settings.RabbitMQPassword);
            if (!rabbitControl.Success)
            {
                monitizer.startSuccesful = -1;
                monitizer.AddException(rabbitControl.Exception, LogTypes.ApplicationError, true);
            }

            ApplicationStartResult mysqlMigrationcontrol = mysql.Migrate(new dao_maindb_context().Database);
            if (!mysqlMigrationcontrol.Success)
            {
                monitizer.startSuccesful = -1;
                monitizer.AddException(mysqlMigrationcontrol.Exception, LogTypes.ApplicationError, true);
            }

            ApplicationStartResult mysqlcontrol = mysql.Connect(_settings.DbConnectionString);
            if (!mysqlcontrol.Success)
            {
                monitizer.startSuccesful = -1;
                monitizer.AddException(mysqlcontrol.Exception, LogTypes.ApplicationError, true);
            }

            TimedEvents.StartTimers();

            if (monitizer.startSuccesful != -1)
            {
                monitizer.startSuccesful = 1;
                monitizer.AddApplicationLog(LogTypes.ApplicationLog, monitizer.appName + " application started successfully.");
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContextPool<dao_maindb_context>(o => o.UseMySQL(_settings.DbConnectionString));            
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();

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

            DefaultFilesOptions DefaultFile = new DefaultFilesOptions();
            DefaultFile.DefaultFileNames.Clear();
            DefaultFile.DefaultFileNames.Add("Index.html");
            app.UseDefaultFiles(DefaultFile);
            app.UseStaticFiles();
        }
    }
}
