﻿using Helpers.Models.DtoModels;
using Helpers.Models.DtoModels.LogDbDto;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.IdentityModels;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;

namespace DAO_IdentityService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet("Login", Name = "Login")]
        public LoginResponse Login(string user, string pass, Helpers.Constants.Enums.AppNames? application, string ip = "", string port = "")
        {
            LoginResponse res = new LoginResponse();

            try
            {
                string json = Helpers.Request.Get(Program._settings.Service_Db_Url + "/users/GetByEmail?email=" + user);

                var userObj = Helpers.Serializers.DeserializeJson<UserDto>(json);

                if (userObj == null || userObj.UserId <= 0)
                {
                    res.IsSuccessful = false;
                    return res;
                }
                else if (Convert.ToBoolean(userObj.IsBlocked))
                {
                    res.IsBlocked = true;
                    res.IsSuccessful = false;

                    Program.monitizer.AddUserLog(userObj.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User login failed. Blocked account.", ip, port);

                    return res;
                }
                else if (!userObj.IsActive)
                {
                    res.IsNotActive = true;
                    res.IsSuccessful = false;

                    Program.monitizer.AddUserLog(userObj.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User login failed. Inactive account.", ip, port);

                    return res;
                }
               
                if (!string.IsNullOrEmpty(pass) && userObj.UserId != 0 && Helpers.Encryption.CheckPassword(userObj.Password, pass))
                {
                    var key = Encoding.ASCII.GetBytes("56753253-tyuw-5769-0921-kdsafirox29zoxqLWERMAwdv");

                    var JWToken = new JwtSecurityToken(
                        claims: GetUserClaims(userObj, UserIdentityType.User),
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                        expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    );

                    var token = new JwtSecurityTokenHandler().WriteToken(JWToken);

                    res.Token = token;
                    res.UserId = userObj.UserId;
                    res.Email = userObj.Email;
                    res.NameSurname = userObj.NameSurname;
                    res.ProfileImage = userObj.ProfileImage;
                    res.IsSuccessful = true;

                    userObj.FailedLoginCount = 0;

                    Program.monitizer.AddUserLog(userObj.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User login successful.", ip, port);

                    //WILL BE INTEGRATED WITH DB (ActiveSessions)
                    //Program.redis.Set("session-" + res.UserId, Helpers.Serializers.Serialize(res));

                    Program.monitizer.AddConsole("User login successful. UserID:" + userObj.UserId);
                }
                else
                {
                    userObj.FailedLoginCount++;

                    Program.monitizer.AddUserLog(userObj.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User login failed. Incorrect password.", ip, port);

                    res.IsSuccessful = false;
                }

                Helpers.Request.Put(Program._settings.Service_Db_Url + "/Users/Update", JsonConvert.SerializeObject(userObj));
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
            }

            return res;

        }

        private IEnumerable<Claim> GetUserClaims(UserDto user, UserIdentityType userType)
        {
            List<Claim> claims = new List<Claim>();

            try
            {
                claims = new List<Claim>
                    {
                        new Claim("Authorization", "Authorized"),
                        new Claim("Newsletter", user.Newsletter.ToString()),
                        new Claim("IsBlocked", user.IsBlocked.ToString()),
                        new Claim("Email", user.Email.ToString()),
                        new Claim("UserId", user.UserId.ToString()),
                    };

                switch (userType)
                {
                    case UserIdentityType.User:

                        return claims;

                    case UserIdentityType.Admin:
                        claims.Add(new Claim("Admin", true.ToString()));
                        return claims;

                    default:
                        return claims;
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
            }

            return claims;
        }

        [HttpPost("Register", Name = "Register")]
        public AjaxResponse Register([FromBody] RegisterModel input)
        {
            try
            {
                UserDto modelUser = new UserDto();
                var jsonUser = Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByEmail?email=" + input.email.ToLower());
                modelUser = Helpers.Serializers.DeserializeJson<UserDto>(jsonUser);
                if (modelUser != null)
                {
                    return new AjaxResponse() { Success = false, Message = "User exist" };
                }

                UserDto model = new UserDto();
                var hashPass = Helpers.Encryption.EncryptPassword(input.password);
                model.Email = input.email.ToLower();
                model.NameSurname = input.namesurname;
                model.Password = hashPass;
                model.Newsletter = false;
                model.IsBlocked = false;
                model.FailedLoginCount = 0;
                model.CreateDate = DateTime.Now;
                model.IsActive = false;
                model.UserType = UserIdentityType.User.ToString();
                model.ProfileImage = "defaultUser.jpg";

                var json = Helpers.Request.Post(Program._settings.Service_Db_Url + "/Users/Post", Helpers.Serializers.SerializeJson(model));
                model = Helpers.Serializers.DeserializeJson<UserDto>(json);
                if (model != null && model.UserId != 0)
                {
                    string enc = Helpers.Encryption.EncryptString(input.email + "|" + DateTime.Now.ToString());
                    string denc = Helpers.Encryption.DecryptString(enc);

                    SendEmailModel emailModel = new SendEmailModel() {  Subject = input.registerEmailTitle, Content = input.registerEmailContent.Replace("{registerbutton}", Program._settings.WebPortal_Url + "/Public/RegisterCompleteView?str=" + enc), To = new List<string> { model.Email } };
                    Helpers.Request.Post(Program._settings.Service_Notification_Url + "/Notification/SendEmail", Helpers.Serializers.SerializeJson(emailModel));
                    
                    Program.monitizer.AddUserLog(model.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User register successful.", input.ip, input.port);

                    return new AjaxResponse() { Success = true };
                }
                else
                {
                    return new AjaxResponse() { Success = false, Message = "User post error" };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new AjaxResponse() { Success = false, Message = "Unexpected error" };
            }

        }

        [HttpGet("RegisterComplete", Name = "RegisterComplete")]
        public AjaxResponse RegisterComplete(string registerCode)
        {
            try
            {
                string stre = Helpers.Encryption.DecryptString(registerCode);

                if (stre.Split('|').Length > 1)
                {
                    string email = stre.Split('|')[0];

                    UserDto modelUser = new UserDto();
                    var jsonUser = Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByEmail?email=" + email.ToLower());
                    modelUser = Helpers.Serializers.DeserializeJson<UserDto>(jsonUser);

                    if (modelUser != null)
                    {
                        modelUser.IsActive = true;
                        Helpers.Request.Put(Program._settings.Service_Db_Url + "/Users/Update", JsonConvert.SerializeObject(modelUser));

                        Program.monitizer.AddUserLog(modelUser.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User account activated.");

                        return new AjaxResponse() { Success = true };
                    }
                    else
                    {
                        return new AjaxResponse() { Success = false };
                    }
                }
                else
                {
                    return new AjaxResponse() { Success = false };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new AjaxResponse() { Success = false };
            }
        }

        [HttpPost("ResetPassword", Name = "ResetPassword")]
        public AjaxResponse ResetPassword(ResetPasswordModel model)
        {
            try
            {
                var userModel = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByEmail?email=" + model.email));
                if (userModel != null && userModel.Email == model.email)
                {
                    string enc = Helpers.Encryption.EncryptString(model.email + "|" + DateTime.Now.ToString());

                    SendEmailModel emailModel = new SendEmailModel() { Subject = model.forgotPassEmailTitle, Content = model.forgotPassEmailContent.Replace("{resetbutton}", Program._settings.WebPortal_Url + "/Public/ResetPasswordView?str=" + enc), To = new List<string> { model.email } };
                    Helpers.Request.Post(Program._settings.Service_Notification_Url + "/Notification/SendEmail", Helpers.Serializers.SerializeJson(emailModel));

                    Program.monitizer.AddUserLog(userModel.UserId, Helpers.Constants.Enums.UserLogType.Auth, "Password reset email sent to user.");

                    return new AjaxResponse { Success = true };
                }
                else
                {
                    return new AjaxResponse { Success = false, Message = "Email error" };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new AjaxResponse() { Success = false };
            }
        }

        [HttpPost("ResetPasswordComplete", Name = "ResetPasswordComplete")]
        public AjaxResponse ResetPasswordComplete(ResetCompleteModel model)
        {
            try
            {
                string tokendec = Helpers.Encryption.DecryptString(model.passwordchangetoken);
                string email = tokendec.Split('|')[0];

                DateTime emaildate = Convert.ToDateTime(tokendec.Split('|')[1]);
                emaildate = emaildate.AddMinutes(5);

                var usr = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByEmail?email=" + email));
                if (usr != null && usr.Email == email && emaildate > DateTime.Now)
                {

                    usr.FailedLoginCount = 0;
                    usr.IsBlocked = false;
                    usr.Password = Helpers.Encryption.EncryptPassword(model.newpass);

                    var userModel = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Put(Program._settings.Service_Db_Url + "/Users/Update", Helpers.Serializers.SerializeJson(usr)));
                    if (userModel != null && userModel.Email == usr.Email)
                    {
                        Program.monitizer.AddUserLog(userModel.UserId, Helpers.Constants.Enums.UserLogType.Auth, "Password reset completed.");

                        return new AjaxResponse { Success = true };
                    }
                    else
                    {
                        return new AjaxResponse { Success = false, Message = "Renew expired" };
                    }
                }
                else
                {
                    return new AjaxResponse { Success = false, Message = "Renew expired" };
                }

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new AjaxResponse() { Success = false };
            }
        }

        [HttpGet("GetUserInfo", Name = "GetUserInfo")]
        public LoginResponse GetUserInfo(string token)
        {
            try
            {
                var parsedToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

                LoginResponse res = new LoginResponse();
                if (parsedToken != null)
                {
                    res.Email = parsedToken.Claims.FirstOrDefault(x => x.Type == "Email").Value;
                    res.UserId = Convert.ToInt32(parsedToken.Claims.FirstOrDefault(x => x.Type == "UserId").Value);
                }
                return res;
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
            }

            return new LoginResponse();
        }

        [HttpGet("Logout", Name = "Logout")]
        public bool Logout(string token)
        {
            try
            {
                //WILL BE INTEGRATED WITH DB (ActiveSessions)

                //var userModel = Helpers.Serializers.DeserializeJson<List<ActiveStatusDto>>(Helpers.Request.Get(Program.settings.AlgolabGetServiceUrl + "/ActiveStatus/GetByToken=" + token)).FirstOrDefault(x => x.Token == token);

                //if (userModel != null)
                //{
                //    DeleteActiveStatus(Convert.ToInt32(user.UserId));
                //}

                //Program.monitizer.AddUserLog(userModel.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User requested logout.");
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
            }
            return true;
        }
    }
}
