using DAO_IdentityService.Controllers;
using FluentAssertions;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.IdentityModels;
using Microsoft.AspNetCore.Mvc.Testing;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;    

namespace DAO_IdentityService.Test
{
    ///<summary>
    /// Tests of DAO_IdentityService.IdentityControllerTests class methods.
    /// IdentityService needes to be connected to RabbitMQ, DAO_DbService, DAO_WebPortal and DAO_Notification service.
    /// Also an EncryptionKey and a JwtTokenKey is needed for Api communication.
    /// Methods:
    /// public LoginResponse Login(LoginModel model)
    /// IEnumerable<Claim> CreateUserClaims(UserDto user, UserIdentityType userType)
    /// SimpleResponse Register([FromBody] RegisterModel registerInput)
    /// SimpleResponse SubmitKYCFile(KYCFileUpload File)
    /// SimpleResponse KycCallBack(KYCCallBack Response)
    /// List<KYCCountries> GetKycCountries()
    /// SimpleResponse RegisterComplete(RegisterCompleteModel model)
    /// SimpleResponse ResetPassword(ResetPasswordModel model)
    /// SimpleResponse ResetPasswordComplete(ResetCompleteModel model)
    /// bool Logout(string token)
    /// </summary>
    public class IdentityControllerTests: IClassFixture<WebApplicationFactory<DAO_IdentityService.Startup>>
    {        
        readonly HttpClient _client;
        private IdentityController controller;
        public static string testPassword;
        ///HttpClient created
        public IdentityControllerTests(WebApplicationFactory<DAO_IdentityService.Startup> fixture)
        {
            _client = fixture.CreateClient();
            controller = new IdentityController();
            testPassword = Guid.NewGuid().ToString("d").Substring(1, 6);
        }

        public interface IRabbitMQConnectionFactory {
            IConnection CreateConnection();
        }
        ///<summary>
        ///User Register Test
        ///Case -1- Register without activation and KYC verification
        ///Case -2- Fail with Email already exists
        ///Case -3- Fail with Username already exsits
        ///</summary>
        [Fact]
        public void Register_Tests(){     

            if(!DAL.ClearUsersTable())
                throw new Exception("MySqlFailure");

            RegisterModel model = new RegisterModel{
                email = "user@mail.com",
                username = "username1", 
                namesurname = "usersurname1",
                password = testPassword,
                ip = "",
                port = ""
            };

            RegisterModel user_with_existing_email = new RegisterModel{
                email = "user@mail.com",
                username = "username_", 
                namesurname = "usersurname1",
                password = testPassword,
                ip = "",
                port = ""
            };

            RegisterModel user_with_existing_username = new RegisterModel{
                email = "user2@mail.com",
                username = "username1", 
                namesurname = "usersurname1",
                password = testPassword,
                ip = "",
                port = ""
            };
            
            var response = controller.Register(model);
            response.Success.Should().Be(true);

            var response2 = controller.Register(user_with_existing_email);
            response2.Success.Should().Be(false);
            response2.Message.Should().Be("Email already exists.");

            var response3 = controller.Register(user_with_existing_username);
            response3.Success.Should().Be(false);
            response3.Message.Should().Be("Username already exists.");            
        }

        /// <summary>
        /// Test of public LoginResponse Login(LoginModel model)
        /// </summary>
        [Fact]
        public void Login_Tests()
        {

            string userJson = string.Empty;
            ///Login model preperation
            LoginModel user1 = new LoginModel() { 
                email = "username6@internal.com",
                pass = "",
                application = Helpers.Constants.Enums.AppNames.DAO_ApiGateway,
                ip = "",
                port =""
            };

            ///-1- Find user from database
            userJson = Helpers.Request.Get(Program._settings.Service_Db_Url + "/users/GetByEmail?email=" + user1.email, Program._settings.JwtTokenKey);
            var userModel1 = Helpers.Serializers.DeserializeJson<UserDto>(userJson);
            
            userModel1.Email.Should().Be("username6@internal.com");

            // var response1 = await _client.GetAsync("http://localhost:8889/users/Get");
            // response1.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void CreateUserClaims(){

        }


    }


}
