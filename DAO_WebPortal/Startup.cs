using Helpers;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static DAO_WebPortal.Program;
using static Helpers.Constants.Enums;

namespace DAO_WebPortal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LoadConfig(configuration);
            InitializeService();

            var config = Configuration.GetSection("PlatformSettings");
            config.Bind(_settings);
        }

        public static void LoadConfig(IConfiguration configuration)
        {
            var config = configuration.GetSection("PlatformSettings");
            config.Bind(_settings);
        }

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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            app.UseHsts();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Public}/{action=Index}/{id?}");
            });
        }
    }
}
