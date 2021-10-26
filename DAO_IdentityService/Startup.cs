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
using static DAO_IdentityService.Program;
using Helpers;
using Helpers.Models.SharedModels;
using System.Text.Json.Serialization;

namespace DAO_IdentityService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LoadConfig(configuration);
            InitializeService();
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
            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            DefaultFilesOptions DefaultFile = new DefaultFilesOptions();
            DefaultFile.DefaultFileNames.Clear();
            DefaultFile.DefaultFileNames.Add("Index.html");
            app.UseDefaultFiles(DefaultFile);
            app.UseStaticFiles();
        }
    }
}
