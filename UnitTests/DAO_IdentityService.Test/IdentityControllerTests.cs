using DAO_IdentityService.Controllers;
// using DAO_DbService.Controllers;
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
using Microsoft.Extensions.Configuration;

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
    /// SimpleResponse RegisterComplete(RegisterCompleteModel model)
    /// SimpleResponse ResetPassword(ResetPasswordModel model)
    /// SimpleResponse ResetPasswordComplete(ResetCompleteModel model)
    /// bool Logout(string token)
    /// </summary>
    public class IdentityControllerTests 
    {        
        private IdentityController identity_controller;
        static string testPassword;
       
        public IdentityControllerTests() 
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            DAO_IdentityService.Startup.LoadConfig(config);
            DAO_IdentityService.Startup.InitializeService();
            identity_controller = new IdentityController();
            testPassword = Guid.NewGuid().ToString("d").Substring(1, 6);
        }        
        ///<summary>
        ///User Register Test
        ///Case -1- Register without activation and KYC verification
        ///Case -2- Fail with Email already exists
        ///</summary>
        [Fact]
        public void Register_Tests(){     

            //Arrange
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
            
            ///Initial register attempt
            ///Incomplete reigster due to pending Email verification
            //Act
            var response = identity_controller.Register(model);
            //Assert
            response.Success.Should().Be(true);

            ///Registration attempt of existing user
            //Act
            var response2 = identity_controller.Register(user_with_existing_email);
            //Assert
            response2.Success.Should().Be(false);
            response2.Message.Should().Be("Email already exists.");          
        }

        /// <summary>
        /// Test of public LoginResponse Login(LoginModel model)
        /// -1- 
        /// </summary>
        [Fact]
        public void Login_Tests()
        {
            //Arrange
            if(!DAL.ClearUsersTable())
                throw new Exception("MySqlFailure"); 

            RegisterModel register_model = new RegisterModel{
                email = "username6@internal.com",
                username = "username6", 
                namesurname = "username_surname",
                password = testPassword,
                ip = "",
                port = ""
            };
            LoginModel login_model = new LoginModel{
                email = "username6@internal.com",
                pass = testPassword,
                application = Helpers.Constants.Enums.AppNames.DAO_ApiGateway,
                ip = "",
                port ="" 
            };            
            LoginModel userX = new LoginModel() { 
                email = "doesnot@exsist.com",
                pass = "",
                application = Helpers.Constants.Enums.AppNames.DAO_ApiGateway,
                ip = "",
                port =""
            };

            ///Act
            var response = identity_controller.Register(register_model);
            ///Assert
            response.Success.Should().Be(true);
            
            string userJson = string.Empty;

            ///Login tests
            ///-1- User existance check control
            userJson = Helpers.Request.Get(Program._settings.Service_Db_Url + "/users/GetByEmail?email=" + login_model.email, Program._settings.JwtTokenKey);
            var userModel1 = Helpers.Serializers.DeserializeJson<UserDto>(userJson);
            
            userModel1.Email.Should().Be("username6@internal.com");

            ///-2- User not found 
            //Act
            LoginResponse result = identity_controller.Login(login_model);
            //Assert
            result.IsSuccessful.Should().Be(false);

            DAL.BlockUser(login_model.email);
            ///-3- User IsBlocked control test (Login fail due to "user is blocked")
            LoginResponse result2 = identity_controller.Login(login_model); 
            result2.IsBlocked.Should().Be(true);
            result2.IsActive.Should().Be(false);

            ///-3- Email activated user account control
            DAL.ResetUser(login_model.email);
            LoginResponse result3 = identity_controller.Login(login_model);
            result3.IsSuccessful.Should().Be(false);
            result3.IsActive.Should().Be(false);            

            ///-4- Successful Login
            DAL.ResetUser(login_model.email);
            DAL.ActivateUser(login_model.email);
            DAL.ActivateKYC(login_model.email);
            LoginResponse result5 = identity_controller.Login(login_model);
            result5.IsSuccessful.Should().Be(true);

            result5.Token.Should().NotBe(null);
            result5.UserId.Should().NotBe(null);
            result5.Email.Should().Be("username6@internal.com");
            result5.NameSurname.Should().Be("username_surname");
            result5.ProfileImage.Should().NotBe(null);
            result5.UserType.Should().Be(Helpers.Constants.Enums.UserIdentityType.Associate);
            result5.IsSuccessful.Should().Be(true);
            result5.IsActive.Should().Be(true);
            result5.IsBanned.Should().Be(false);
            result5.IsBlocked.Should().Be(false);  
        }
    }


}
