using Helpers.Constants;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.IdentityModels;
using Helpers.Models.KYCModels;
using Helpers.Models.NotificationModels;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static Helpers.Constants.Enums;

namespace DAO_IdentityService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        ///  User login method with email or username.
        ///  A JWT Token is created with every successful login. Active user sessions are stored in the database (ActiveSessions table)
        ///  It is recommended to change the JwtTokenKey in production environment.
        /// </summary>
        /// <param name="model">LoginModel</param>
        /// <returns>User summary and token</returns>
        [HttpPost("Login", Name = "Login")]
        public LoginResponse Login(LoginModel model)
        {
            LoginResponse res = new LoginResponse();

            try
            {
                //Try to find user in database
                string userJson = string.Empty;
                //Try to find user with email
                if (model.email.Contains("@"))
                {
                    userJson = Helpers.Request.Get(Program._settings.Service_Db_Url + "/users/GetByEmail?email=" + model.email);
                }
                //Try to find user with username
                else
                {
                    userJson = Helpers.Request.Get(Program._settings.Service_Db_Url + "/users/GetByUsername?username=" + model.email);
                }
                var userObj = Helpers.Serializers.DeserializeJson<UserDto>(userJson);

                //User not found control
                if (userObj == null || userObj.UserId <= 0)
                {
                    res.IsSuccessful = false;
                    return res;
                }

                //User blocked(banned) control
                if (Convert.ToBoolean(userObj.IsBlocked))
                {
                    res.IsBlocked = true;
                    res.IsSuccessful = false;

                    Program.monitizer.AddUserLog(userObj.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User login failed. Blocked account.", model.ip, model.port);

                    return res;
                }

                //User email activation control
                if (!userObj.IsActive)
                {
                    res.IsActive = false;
                    res.IsSuccessful = false;

                    Program.monitizer.AddUserLog(userObj.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User login failed. Inactive account.", model.ip, model.port);

                    return res;
                }

                //Password control
                if (string.IsNullOrEmpty(model.pass) || !Helpers.Encryption.CheckPassword(userObj.Password, model.pass))
                {
                    res.IsSuccessful = false;

                    Program.monitizer.AddUserLog(userObj.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User login failed. Incorrect password.", model.ip, model.port);

                    return res;
                }

                //--------LOGIN SUCCESFUL--------

                //Create JWT Token
                var key = Encoding.ASCII.GetBytes(Program._settings.JwtTokenKey);

                var JWToken = new JwtSecurityToken(
                    claims: CreateUserClaims(userObj, (Enums.UserIdentityType)Enum.Parse(typeof(Enums.UserIdentityType), userObj.UserType)),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                );

                var token = new JwtSecurityTokenHandler().WriteToken(JWToken);

                //Create response object
                res.Token = token;
                res.UserId = userObj.UserId;
                res.Email = userObj.Email;
                res.NameSurname = userObj.NameSurname;
                res.ProfileImage = userObj.ProfileImage;
                res.UserType = (Enums.UserIdentityType)Enum.Parse(typeof(Enums.UserIdentityType), userObj.UserType);
                res.IsSuccessful = true;
                res.IsActive = true;
                res.IsBanned = false;
                res.IsBlocked = false;
                res.KYCStatus = userObj.KYCStatus;

                //Post or update user session in database
                Helpers.Request.Post(Program._settings.Service_Db_Url + "/ActiveSession/PostOrUpdate", Helpers.Serializers.SerializeJson(new ActiveSessionDto() { LoginDate = DateTime.Now, Token = token, UserID = res.UserId }));

                //Logging
                Program.monitizer.AddUserLog(userObj.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User login successful.", model.ip, model.port);
                Program.monitizer.AddConsole("User login successful. UserID:" + userObj.UserId);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
            }

            return res;
        }

        /// <summary>
        ///  Get claims of the user from user object and user identity type
        /// </summary>
        /// <param name="user">UserDto object</param>
        /// <param name="userType">UserIdentityType enum</param>
        /// <returns>User claims list</returns>
        private IEnumerable<Claim> CreateUserClaims(UserDto user, UserIdentityType userType)
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
                    case UserIdentityType.Associate:
                        return claims;
                    case UserIdentityType.Admin:
                        claims.Add(new Claim("Admin", true.ToString()));
                        return claims;
                    case UserIdentityType.VotingAssociate:
                        claims.Add(new Claim("Associate", true.ToString()));
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

        /// <summary>
        ///  User register method
        /// </summary>
        /// <param name="registerInput">Registration information of the user</param>
        /// <returns>Generic SimpleResponse class</returns>
        [HttpPost("Register", Name = "Register")]
        public SimpleResponse Register([FromBody] RegisterModel registerInput)
        {
            try
            {
                //Email already exists control
                UserDto userControlModel1 = new UserDto();
                var userControlJson1 = Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByEmail?email=" + registerInput.email.ToLower());
                userControlModel1 = Helpers.Serializers.DeserializeJson<UserDto>(userControlJson1);
                if (userControlModel1 != null)
                {
                    return new SimpleResponse() { Success = false, Message = "Email already exists." };
                }

                //Username already exists control
                UserDto userControlModel2 = new UserDto();
                var userControlJson2 = Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByUsername?username=" + registerInput.username.ToLower());
                userControlModel2 = Helpers.Serializers.DeserializeJson<UserDto>(userControlJson2);
                if (userControlModel2 != null)
                {
                    return new SimpleResponse() { Success = false, Message = "Username already exists." };
                }

                //Create new user object
                UserDto userModel = new UserDto();
                var hashPass = Helpers.Encryption.EncryptPassword(registerInput.password);
                userModel.Email = registerInput.email.ToLower();
                userModel.UserName = registerInput.username;
                userModel.NameSurname = registerInput.namesurname;
                userModel.Password = hashPass;
                userModel.Newsletter = false;
                userModel.IsBlocked = false;
                userModel.FailedLoginCount = 0;
                userModel.CreateDate = DateTime.Now;
                userModel.IsActive = false;   //Should be false in the production environment for email approval.
                userModel.UserType = UserIdentityType.Associate.ToString();
                userModel.ProfileImage = "default.png";

                //Insert user object to database
                var json = Helpers.Request.Post(Program._settings.Service_Db_Url + "/Users/Post", Helpers.Serializers.SerializeJson(userModel));
                userModel = Helpers.Serializers.DeserializeJson<UserDto>(json);
                if (userModel != null && userModel.UserId != 0)
                {

                    //Create encrypted activation key for email approval
                    string enc = Helpers.Encryption.EncryptString(registerInput.email + "|" + DateTime.Now.ToString());

                    //Set email title and content
                    string emailTitle = "Welcome to " + Program._settings.DAOName;
                    string emailContent = "Greetings " + userModel.NameSurname.Split(' ')[0] + ", <br><br> Please use the link below to complete your registration. <br><br>" + "<a href='" + Program._settings.WebPortal_Url + "/Public/RegisterCompleteView?str=" + enc + "'>Click here to complete the registration.</a>";

                    //Send email
                    SendEmailModel emailModel = new SendEmailModel() { Subject = emailTitle, Content = emailContent, To = new List<string> { userModel.Email } };
                    Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));


                    //Logging
                    Program.monitizer.AddUserLog(userModel.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User register successful.", registerInput.ip, registerInput.port);


                    return new SimpleResponse() { Success = true };
                }
                else
                {
                    return new SimpleResponse() { Success = false, Message = "User post error" };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new SimpleResponse() { Success = false, Message = "Unexpected error" };
            }

        }

        /// <summary>
        ///  Email approval method after registration
        /// </summary>
        /// <param name="model">Token generated from the Register method</param>
        /// <returns>Generic SimpleResponse class</returns>
        [HttpPost("RegisterComplete", Name = "RegisterComplete")]
        public SimpleResponse RegisterComplete(RegisterCompleteModel model)
        {
            try
            {
                //Decrypt token in the email
                string stre = Helpers.Encryption.DecryptString(model.registerToken);

                //Check if it's a valid token
                if (stre.Split('|').Length > 1)
                {
                    string email = stre.Split('|')[0];

                    //Find user in database
                    UserDto modelUser = new UserDto();
                    var jsonUser = Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByEmail?email=" + email.ToLower());
                    modelUser = Helpers.Serializers.DeserializeJson<UserDto>(jsonUser);

                    if (modelUser != null)
                    {
                        //Change active status of the user and update database
                        modelUser.IsActive = true;
                        Helpers.Request.Put(Program._settings.Service_Db_Url + "/Users/Update", JsonConvert.SerializeObject(modelUser));

                        //Logging
                        Program.monitizer.AddUserLog(modelUser.UserId, Helpers.Constants.Enums.UserLogType.Auth, "User account activated.");
                        return new SimpleResponse() { Success = true };
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
            }

            return new SimpleResponse() { Success = false };
        }

        /// <summary>
        ///  Reset password request method
        /// </summary>
        /// <param name="model">User registered email in the system</param>
        /// <returns>Generic SimpleResponse class</returns>
        [HttpPost("ResetPassword", Name = "ResetPassword")]
        public SimpleResponse ResetPassword(ResetPasswordModel model)
        {
            try
            {
                //Find user in database from email
                var userModel = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByEmail?email=" + model.email));

                //Check user exists
                if (userModel != null)
                {
                    //Create encrypted token for password renewal
                    string enc = Helpers.Encryption.EncryptString(model.email + "|" + DateTime.Now.ToString());

                    //Set password renewal email title and content
                    string emailTitle = Program._settings.DAOName + " Password Renewal";
                    string emailContent = "Greetings " + userModel.NameSurname.Split(' ')[0] + ", <br><br> Please use the link below to reset your password. <br><br>" + "<a href='" + Program._settings.WebPortal_Url + "/Public/ResetPasswordView?str=" + enc + "'>Click here to reset your password.</a>";

                    //Send password renewal email
                    SendEmailModel emailModel = new SendEmailModel() { Subject = emailTitle, Content = emailContent, To = new List<string> { model.email } };
                    Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));

                    //Logging
                    Program.monitizer.AddUserLog(userModel.UserId, Helpers.Constants.Enums.UserLogType.Auth, "Password reset email sent to user.");

                    return new SimpleResponse { Success = true };
                }
                else
                {
                    return new SimpleResponse { Success = false, Message = "Email not found" };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new SimpleResponse() { Success = false };
            }
        }

        /// <summary>
        ///  Change password method after reset password request
        /// </summary>
        /// <param name="model">passwordChangeToken: Token generated from ResetPassword method</param>
        /// <returns>Generic SimpleResponse class</returns>
        [HttpPost("ResetPasswordComplete", Name = "ResetPasswordComplete")]
        public SimpleResponse ResetPasswordComplete(ResetCompleteModel model)
        {
            try
            {
                //Decrypt token in password renewal email
                string tokendec = Helpers.Encryption.DecryptString(model.passwordChangeToken);
                string email = tokendec.Split('|')[0];

                //Find user in database
                var usr = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetByEmail?email=" + email));

                DateTime emaildate = Convert.ToDateTime(tokendec.Split('|')[1]);
                emaildate = emaildate.AddDays(5);

                //Check if user is valid and password renewal is expired
                if (usr != null && usr.Email == email && emaildate > DateTime.Now)
                {
                    //Reset password
                    usr.Password = Helpers.Encryption.EncryptPassword(model.newPass);

                    //Update user password in database
                    var userModel = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Put(Program._settings.Service_Db_Url + "/Users/Update", Helpers.Serializers.SerializeJson(usr)));

                    //Logging
                    Program.monitizer.AddUserLog(userModel.UserId, Helpers.Constants.Enums.UserLogType.Auth, "Password reset completed.");

                    return new SimpleResponse { Success = true, Message = "Password reset completed." };
                }
                else
                {
                    //Logging
                    Program.monitizer.AddApplicationLog(LogTypes.PublicUserLog, "Password renewal request failed. Email Date:" +emaildate.ToString()+ "  DateNow:" +DateTime.Now.ToString()+" "+ usr.Email);

                    return new SimpleResponse { Success = false, Message = "Renew expired" };
                }

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new SimpleResponse() { Success = false };
            }
        }

        /// <summary>
        ///  User logout method
        /// </summary>
        /// <param name="token">Jwt token of the user</param>
        /// <returns>Is successful</returns>
        [HttpGet("Logout", Name = "Logout")]
        public bool Logout(string token)
        {
            try
            {
                //Get UserId from JWT token
                var tokenObj = new JwtSecurityTokenHandler().ReadJwtToken(token);
                int userId = Convert.ToInt32(tokenObj.Claims.First(c => c.Type == "UserId").Value);

                //Delete all active sessions of the user from database
                Helpers.Request.Delete(Program._settings.Service_Db_Url + "/ActiveSession/DeleteByUserId?userid=" + userId);

                //Logging
                Program.monitizer.AddUserLog(userId, Helpers.Constants.Enums.UserLogType.Auth, "User requested logout.");
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
            }
            return true;
        }
    }
}
