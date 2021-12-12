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
        ///  Submits form data for the KYC verification
        /// </summary>
        /// <param>User information</param>
        /// <returns>Generic Simple Response class</returns>
        [HttpPost("SubmitKYCFile", Name = "SubmitKYCFile")]
        public SimpleResponse SubmitKYCFile([FromQuery] string Type, string Name, string Surname, string DoB, string Email, string Country, string DocumentNumber, string IssueDate, string ExpiryDate, string UserID)
        {
            try
            {
                var userModel = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/UserKYC/GetUserId?id=" + UserID));

                if (userModel == null || userModel.UserKYCID <= 0)
                {
                    //Create KYC applicant
                    var Person = new KYCPerson() { type = "PERSON", first_name = Name, last_name = Surname, dob = DoB, residence_country = Country, email = Email };
                    var model = Helpers.Serializers.DeserializeJson<KYCPersonResponse>(Helpers.Request.KYCPost(Program._settings.KYCURL + "/applicants", Helpers.Serializers.SerializeJson(Person), Program._settings.KYCID));

                    if (model != null)
                    {
                        if (Request.HasFormContentType)
                        {
                            Guid g = Guid.NewGuid();

                            var form = Request.Form;

                            if (form.Files.Count > 0)
                            {
                                //Create File Request
                                var model2 = Helpers.Request.UploadFiletoKYCAID(Program._settings.KYCURL + "/files", form.Files[0], Program._settings.KYCID);

                                //Create applicant document request
                                var Doc = new KYCDocument() { applicant_id = model.applicant_id, type = Type, document_number = DocumentNumber, issue_date = IssueDate, expiry_date = ExpiryDate, front_side_id = model2.file_id };


                                KYCFileResponse model3 = new KYCFileResponse();
                                if (form.Files.Count > 1)
                                {
                                    model3 = Helpers.Request.UploadFiletoKYCAID(Program._settings.KYCURL + "/files", form.Files[1], Program._settings.KYCID);

                                    Doc.back_side_id = model3.file_id;
                                }


                                var model4 = Helpers.Serializers.DeserializeJson<KYCDocumentResponse>(Helpers.Request.KYCPost(Program._settings.KYCURL + "/documents", Helpers.Serializers.SerializeJson(Doc), Program._settings.KYCID));

                                //Create verification request

                                var Verify = new KYCVerification() { applicant_id = model.applicant_id, types = new List<string>() { "DOCUMENT" }, callback_url = Program._settings.WebURLforKYCResponse };
                                var model5 = Helpers.Serializers.DeserializeJson<KYCVerificationResponse>(Helpers.Request.KYCPost(Program._settings.KYCURL + "/verifications", Helpers.Serializers.SerializeJson(Verify), Program._settings.KYCID));

                                //New KYC request for Db record
                                var NewKYCObj = new UserKYCDto() { UserID = Convert.ToInt32(UserID), ApplicantId = model.applicant_id, VerificationId = model5.verification_id, FileId1 = model2.file_id, KYCStatus = "pending" ,DocumentId=model4.document_id};

                                if (form.Files.Count > 1)
                                    NewKYCObj.FileId2 = model3.file_id;

                                var NewKYC = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Post(Program._settings.Service_Db_Url + "/UserKYC/Post", Helpers.Serializers.SerializeJson(NewKYCObj)));

                                if (NewKYC.UserKYCID <= 0)
                                {
                                    return new SimpleResponse() { Success = false, Message = "KYC error" };
                                }
                            }
                            else
                            {
                                return new SimpleResponse() { Success = false, Message = "Please upload KYC Document." };
                            }
                        }
                        else
                        {
                            return new SimpleResponse() { Success = false, Message = "Please upload KYC Document." };
                        }
                    }
                    else
                    {
                        Program.monitizer.AddConsole("KYCAID connection error.");
                        return new SimpleResponse() { Success = false, Message = "An error occurred during operations. Please try again later." };
                    }

                }
                else
                {
                    //update applicant

                    var Person = new KYCPerson() { type = "PERSON", first_name = Name, last_name = Surname, dob = DoB, residence_country = Country, email = Email };
                    var model = Helpers.Serializers.DeserializeJson<KYCPersonResponse>(Helpers.Request.KYCPatch(Program._settings.KYCURL + "/applicants/" + userModel.ApplicantId, Helpers.Serializers.SerializeJson(Person), Program._settings.KYCID));
                    if (Request.HasFormContentType)
                    {
                        Guid g = Guid.NewGuid();

                        var form = Request.Form;

                        if (form.Files.Count > 0)
                        {
                            //Update File 
                            var model2 = Helpers.Request.PutFiletoKYCAID(Program._settings.KYCURL + "/files/"+userModel.FileId1, form.Files[0], Program._settings.KYCID);

                            //Update applicant document request
                            var Doc = new KYCDocument() { applicant_id = model.applicant_id, type = Type, document_number = DocumentNumber, issue_date = IssueDate, expiry_date = ExpiryDate, front_side_id = model2.file_id };


                            KYCFileResponse model3 = new KYCFileResponse();
                            if (form.Files.Count > 1)
                            {
                                if(userModel.FileId2 == null)
                                {
                                    model3 = Helpers.Request.UploadFiletoKYCAID(Program._settings.KYCURL + "/files", form.Files[1], Program._settings.KYCID);
                                }
                                else
                                {
                                    model3 = Helpers.Request.PutFiletoKYCAID(Program._settings.KYCURL + "/files/" + userModel.FileId2, form.Files[0], Program._settings.KYCID);
                                }

                                Doc.back_side_id = model3.file_id;
                            }

                            var model4 = Helpers.Serializers.DeserializeJson<KYCDocumentResponse>(Helpers.Request.KYCPatch(Program._settings.KYCURL + "/documents/"+userModel.DocumentId, Helpers.Serializers.SerializeJson(Doc), Program._settings.KYCID));

                            var Verify = new KYCVerification() { applicant_id = model.applicant_id, types = new List<string>() { "DOCUMENT" }, callback_url = Program._settings.WebURLforKYCResponse };
                            var model5 = Helpers.Serializers.DeserializeJson<KYCVerificationResponse>(Helpers.Request.KYCPost(Program._settings.KYCURL + "/verifications", Helpers.Serializers.SerializeJson(Verify), Program._settings.KYCID));

                            //Update KYC Db record
                            var NewKYCObj = new UserKYCDto() { UserID = Convert.ToInt32(UserID), ApplicantId = model.applicant_id, VerificationId = model5.verification_id, FileId1 = model2.file_id, KYCStatus = "pending", DocumentId = model4.document_id ,UserKYCID = userModel.UserKYCID};

                            if (form.Files.Count > 1)
                                NewKYCObj.FileId2 = model3.file_id;

                            var NewKYC = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Put(Program._settings.Service_Db_Url + "/UserKYC/Update", Helpers.Serializers.SerializeJson(NewKYCObj)));

                            if (NewKYC.UserKYCID <= 0)
                            {
                                return new SimpleResponse() { Success = false, Message = "KYC update error" };
                            }
                        }
                        else
                        {
                            return new SimpleResponse() { Success = false, Message = "Please upload KYC Document." };
                        }
                    }
                    else
                    {
                        return new SimpleResponse() { Success = false, Message = "Please upload KYC Document." };
                    }


                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new SimpleResponse() { Success = false, Message = "Unexpected error" };
            }
            return new SimpleResponse() { Success = true , Message = "KYC completed successfully." };
        }


        /// <summary>
        ///  Receives calllback from the KYC verification site
        /// </summary>
        /// <param name="Response">KYC information of the user</param>
        /// <returns>KYCCallBack class</returns>
        [HttpPost("KycCallBack", Name = "KycCallBack")]
        public SimpleResponse KycCallBack(KYCCallBack Response)
        {
            Program.monitizer.AddConsole(Response.applicant_id);

            try
            {
                var userModel = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/UserKYC/GetApplicantId?id=" + Response.applicant_id));
                if (userModel != null )
                {
                    userModel.Comment = Response.verifications.document.comment;
                    userModel.Verified = Response.verified;
                    userModel.KYCStatus = Response.status;

                    var KYCModel = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Put(Program._settings.Service_Db_Url + "/UserKYC/Update", Helpers.Serializers.SerializeJson(userModel)));
                    if (KYCModel.UserKYCID <= 0)
                    {
                        Program.monitizer.AddConsole("KYC Update error");
                        return new SimpleResponse() { Success = false, Message = "KYC Update error" };
                    }

                    if (Response.verified)
                    {
                        var User = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetId?Id=" + userModel.UserID));
                        if (User == null)
                        {
                            Program.monitizer.AddConsole("User not found");
                            return new SimpleResponse() { Success = false, Message = "User not found" };
                        }
                        else
                        {
                            User.KYCStatus = true;
                            var UserUpdate = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Put(Program._settings.Service_Db_Url + "/Users/Update", Helpers.Serializers.SerializeJson(User)));
                            if (UserUpdate != null && UserUpdate.UserId > 0)
                            {
                                return new SimpleResponse() { Success = true };
                            }
                            else
                            {
                                return new SimpleResponse() { Success = false, Message = "User failed to update" };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new SimpleResponse() { Success = false, Message = "Unexpected error" };
            }
            return new SimpleResponse() { Success = true  };

        }

        /// <summary>
        /// Brings countries for KYC form 
        /// </summary>
        /// <returns>List of KYCCountries class</returns>
        [HttpPost("GetKycCountries", Name = "GetKycCountries")]
        public List<KYCCountries> GetKycCountries()
        {
            List<KYCCountries> CountryList = new List<KYCCountries>();
            try
            {
                CountryList = Helpers.Serializers.DeserializeJson<List<KYCCountries>>(Helpers.Request.KYCGet(Program._settings.KYCURL + "/countries", Program._settings.KYCID));
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new List<KYCCountries>();
            }
            return CountryList;

        }


        /// <summary>
        /// Brings KYC status of user 
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>UserKYCDto class</returns>
        [HttpGet("GetKycStatus", Name = "GetKycStatus")]
        public UserKYCDto GetKycStatus(int id)
        {
   
            UserKYCDto model = new UserKYCDto();
            try
            {
               model = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/UserKYC/GetUserId?id=" + id, Helpers.Serializers.SerializeJson(User)));
                if (model == null)
                    model = new UserKYCDto();
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new UserKYCDto();
            }
            return model;
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
                emaildate = emaildate.AddMinutes(60);

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
