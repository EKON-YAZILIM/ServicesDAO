using Microsoft.VisualBasic.CompilerServices;
using System;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAO_WebPortal.Test.Util;
using Xunit.Sdk;
using Microsoft.Extensions.Configuration;
using Helpers.Models.IdentityModels;
using DAO_NotificationService.Controllers;
using DAO_LogService.Controllers;
using DAO_IdentityService.Controllers;
using DAO_WebPortal.Controllers;
using Helpers.Models.SharedModels;
using FluentAssertions;
using DAO_LogService.Models;
using System.Collections.Generic;
using PagedList.Core;
using System.Linq;

namespace DAO_WebPortal.Test
{
    [Collection("Sequential")]
    /// <summary>
    /// Below class tests critical functions of DAO_WebPortal Controller.
    /// Due microservice architecture structure testing DAO_WebPortal controller methods also tests the DAO_LogService, DAO_NotificationService, 
    /// RabbitMQ functions and controller functions and DAO_ApiGateway.
    /// </summary>
    public class WebPortalTests
    {
        /// <summary>
        /// Creates instance of ISession to be build as mock session
        /// </summary>
        ISession session;
        /// <summary>
        /// Creating instances of 
        ///     DAO_WebPortal.Controllers.HomeController 
        ///     DAO_IdentityService.Controllers.IdentityController
        ///     DAO_NotificationService.Controllers.NotificationController        
        ///     DAO_LogService.Controllers.UserLogController
        ///     DAO_LogService.Controllers.ErrorLogController
        ///for testing
        /// </summary>
        private HomeController homecontroller;
        private IdentityController identity_controller;
        private NotificationController notificationController;
        private UserLogController user_logController;
        private ErrorLogController error_logController;

        RegisterModel register_model = new RegisterModel
        {
            email = "username6@internal.com",
            username = "username6",
            namesurname = "username_surname",
            password = "124qwe",
            ip = "",
            port = ""
        };

        LoginModel login_model = new LoginModel
        {
            email = "username6@internal.com",
            pass = "124qwe",
            application = Helpers.Constants.Enums.AppNames.DAO_ApiGateway,
            ip = "",
            port = ""
        };
        
        /// <summary>
        /// Initailizes microservices controllers, databases and http session
        /// </summary>
        public WebPortalTests(){
            var identity_config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            DAO_IdentityService.Startup.LoadConfig(identity_config);
            DAO_IdentityService.Startup.InitializeService();
            identity_controller = new IdentityController();

            var notification_config = new ConfigurationBuilder().AddJsonFile("appsettings.notiftests.json").Build();
            DAO_NotificationService.Startup.LoadConfig(notification_config);
            DAO_NotificationService.Startup.InitializeService();
            notificationController = new NotificationController();

            var logservice_conf = new ConfigurationBuilder().AddJsonFile("appsettings.logtests.json").Build();
            DAO_LogService.Startup.LoadConfig(logservice_conf);
            DAO_LogService.Startup.InitializeService();
            user_logController = new UserLogController();
            error_logController = new ErrorLogController();

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.tests.json").Build();
            DAO_WebPortal.Startup.LoadConfig(config);
            DAO_WebPortal.Startup.InitializeService();
            homecontroller = new HomeController();
            homecontroller.ControllerContext = new ControllerContext();
            homecontroller.ControllerContext.HttpContext = new DefaultHttpContext();
            session = homecontroller.ControllerContext.HttpContext.Session = new MockHttpSession();
        }

        /// <summary>
        /// Test of Identity service login and communication between WebPortal via ApiGateway.
        /// Test of posting a new job thru ApiGateway
        /// Test of logging various user and application operations and errors
        /// Test of Notification service and RabbitMQ functions
        /// </summary>
        [Fact]
        public void New_Job_Post_Test()
        {
            ///Arrange
            ///Clears the database for registering a new user 
            if (!(DAL.ClearUsersTable() && DAL.ClearLogTables()))
                throw new Exception("MySqlFailure");
            
            ///Act: Successful register attempt.
            var response = identity_controller.Register(register_model);
            ///Assert 1
            response.Success.Should().Be(true);

            LoginResponse login_result = new LoginResponse();
            if (response.Success)
            {
                DAL.ActivateUser(login_model.email);
                DAL.ActivateKYC(login_model.email);

                login_result = identity_controller.Login(login_model);
            }

            session.SetString("UserType", login_result.UserType.ToString());
            session.SetInt32("UserID", login_result.UserId);
            session.SetString("Token", login_result.Token);
            homecontroller.ControllerContext.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("122.122.122.1");
            homecontroller.ControllerContext.HttpContext.Connection.RemotePort = 9948;
            var t = homecontroller.New_Job_Post("title", 5000, "16:44", "description", "tags",  "codeurl");

            SimpleResponse resp = (SimpleResponse)t.Value;

            resp.Success.Should().Be(true);
        }

        [Fact]
        public void NotificationTest(){
                        
            List<object> userlist = DAL.GetLogsList("UserLogs");   
            List<UserLog> userlogList = new List<UserLog>();         

            foreach(var element in userlist){
                userlogList.Add((UserLog)element);
            }
            userlogList.OrderBy(a=>a.UserLogId);
            (userlogList.ElementAt(1).Explanation == "User register successful." || userlogList.ElementAt(1).Explanation == "User login successful.").Should().Be(true);
            //userlogList.ElementAt(1).Explanation.Should().Be("User register successful.");
            //userlogList.ElementAt(0).Explanation.Should().Be("User login successful.");

            List<object> errorlist = DAL.GetLogsList("ErrorLogs");   
            errorlist.Count.Should().Be(0);

            List<object> applist = DAL.GetLogsList("ApplicationLogs");   
            List<ApplicationLog> applogList = new List<ApplicationLog>();         

            foreach(var element in applist){
                applogList.Add((ApplicationLog)element);
            }

            ApplicationLog applog = applogList.Where(el => el.Application == "DAO_NotificationService").FirstOrDefault();
            
            //userlogList.ElementAt(1).Explanation.Should().Be("Email notification sent. {"Subject":"Welcome to ServicesDAO","Content":"Greetings username_surname, <br><br> Please use the link below to complete your registration. <br><br><a href='http://localhost:8895/Public/RegisterCompleteView?str=8SpVEMYjiq4OYIniqlC9vY2jEOkr1Ka5OClsKO28i6rEMyhD5fONtCdSLsBlwsYGPV8WIZ5mN6TNbTkhw4VRQi8pM+AdFdpnBc3Veb24SedAZR5UxiB2Fv5o1BzRq4Hm'>Click here to complete the registration.</a>","To":["username6@internal.com"],"Cc":[],"Bcc":[],"TargetGroup":null}");
            applog.Explanation.Should().Contain("Email notification sent.");

        }



    }
}
