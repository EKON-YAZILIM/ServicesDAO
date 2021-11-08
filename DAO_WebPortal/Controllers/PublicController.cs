using DAO_WebPortal.Utility;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAO_WebPortal.Resources;
using Helpers.Models.IdentityModels;
using static Helpers.Constants.Enums;
using System.Text.RegularExpressions;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.NotificationModels;

namespace DAO_WebPortal.Controllers
{
    /// <summary>
    ///  Controller for public views and public actions
    /// </summary>
    public class PublicController : Controller
    {
        #region Views
        public IActionResult Index()
        {
            return View();
        }

        [Route("Privacy-Policy")]
        public IActionResult Privacy_Policy()
        {
            ViewBag.HeaderTitle = "Privacy Policy";
            ViewBag.HeaderSubTitle = "ServicesDAO privacy policy and user agreement";


            return View();
        }

        [Route("Contact")]
        public IActionResult Contact()
        {
            ViewBag.HeaderTitle = "Contact";
            ViewBag.HeaderSubTitle = "Feel free to reach out for any questions and wishes.";

            return View();
        }

        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }

        #endregion

        /// <summary>
        ///  Sends user's message as email to system admins
        /// </summary>
        /// <param name="namesurname">User's name surname</param>
        /// <param name="email">User's email</param>
        /// <param name="message">User's message</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SubmitContactForm(string namesurname, string email, string message, string usercode)
        {
            try
            {
                // Check captcha
                if (!Utility.Captcha.ValidateCaptchaCode("securityCodeContact", usercode, HttpContext))
                {
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                //Create email model
                SendEmailModel model = new SendEmailModel();
                model.Subject = "Contact form submission from anonymous user";
                model.Content = "Name surname: " + namesurname + ", Email:" + email + ", Message:" + message;

                //Send email to system Admin
                string jsonResponse = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Notification/SendPublicContactEmail", Helpers.Serializers.SerializeJson(model));

                //Parse response
                SimpleResponse res = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResponse);

                if(res.Success == false)
                {
                    res.Message = "Currently we are unable to send your message. Please try again later.";
                }

                return Json(res);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
            }
        }

        #region Login & Register Methods

        /// <summary>
        ///  User login function
        /// </summary>
        /// <param name="email">User's email or username</param>
        /// <param name="password">User's password</param>
        /// <param name="usercode">Captcha code (Needed after 3 failed requests)</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login(string email, string password, string usercode)
        {
            try
            {
                // Check captcha only after 3 failed requests
                int failCount = Convert.ToInt32(HttpContext.Session.GetInt32("FailCount"));
                if (failCount > 3 && !Utility.Captcha.ValidateCaptchaCode("securityCodeLogin", usercode, HttpContext))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                //Get client Ip and Port
                string ip = IpHelper.GetClientIpAddress(HttpContext);
                string port = IpHelper.GetClientPort(HttpContext);

                //Create model
                LoginModel LoginModelPost = new LoginModel() { email = email, pass = password, ip = ip, port = port, application = Helpers.Constants.Enums.AppNames.DAO_WebPortal };

                //Post model to ApiGateway
                var loginJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/Login", Helpers.Serializers.SerializeJson(LoginModelPost));

                //Parse response
                LoginResponse loginModel = Helpers.Serializers.DeserializeJson<LoginResponse>(loginJson);

                if (loginModel.UserId != 0 && loginModel != null && loginModel.IsSuccessful == true)
                {
                    string token = loginModel.Token.ToString();
                    string login_email = loginModel.Email.ToString();

                    HttpContext.Session.SetInt32("UserID", loginModel.UserId);
                    HttpContext.Session.SetString("Email", login_email);
                    HttpContext.Session.SetString("Token", token);
                    HttpContext.Session.SetString("LoginType", "user");
                    HttpContext.Session.SetString("NameSurname", loginModel.NameSurname);
                    HttpContext.Session.SetString("UserType", loginModel.UserType.ToString());
                    HttpContext.Session.SetString("ProfileImage", loginModel.ProfileImage);

                    return base.Json(new SimpleResponse { Success = true, Message = Lang.SuccessLogin });
                }
                else
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorUsernamePassword });
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
            }
        }

        /// <summary>
        ///  New user registration function
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="username">Username</param>
        /// <param name="namesurname">Name Surname</param>
        /// <param name="password">Password</param>
        /// <param name="repass">Password confirmation</param>
        /// <param name="usercode">Captcha code (Needed after 3 failed requests)</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Register(string email, string username, string namesurname, string password, string repass, string usercode)
        {
            try
            {
                // Check captcha only after 3 failed requests
                int failCount = Convert.ToInt32(HttpContext.Session.GetInt32("FailCount"));
                if (failCount > 3 && !Captcha.ValidateCaptchaCode("securityCodeRegister", usercode, HttpContext))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                //Password match control
                if (password != repass)
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.NotCompatiblePass });
                }

                //Password strength control
                if (!Regex.IsMatch(password, @"^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])"))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorPasswordMsg });
                }

                //Get client Ip and Port
                string ip = IpHelper.GetClientIpAddress(HttpContext);
                string port = IpHelper.GetClientPort(HttpContext);

                //Post model to ApiGateway
                var registerJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/Register", Helpers.Serializers.SerializeJson(new RegisterModel() { email = email, username = username, namesurname = namesurname, password = password, ip = ip, port = port }));

                //Parse response
                SimpleResponse registerResponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(registerJson);

                if (registerResponse.Success == false)
                {
                    failCount++;
                    if (registerResponse.Message == "Username already exists.")
                    {
                        return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorUserMsg });
                    }
                    else if (registerResponse.Message == "Email already exists.")
                    {
                        return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorMailMsg });
                    }
                    else if (registerResponse.Message == "User post error")
                    {
                        return base.Json(new SimpleResponse { Success = false, Message = Lang.UnexpectedError });
                    }
                    else
                    {
                        return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
                    }
                }
                else
                {
                    return base.Json(new SimpleResponse { Success = true, Message = Lang.RegisterEmailSent });
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
            }
        }

        /// <summary>
        /// Completes user registration from activation link in the confirmation email
        /// </summary>
        /// <param name="str">Encrypted user information in the registration email</param>
        /// <returns></returns>
        public ActionResult RegisterCompleteView(string str)
        {
            try
            {
                //Get result
                var completeJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/RegisterComplete", Helpers.Serializers.SerializeJson(new Helpers.Models.IdentityModels.RegisterCompleteModel() { registerToken = str }));

                //Parse result
                SimpleResponse completeResponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(completeJson);

                if (completeResponse.Success)
                {
                    TempData["message"] = Lang.RegisterComplete;
                }
                else
                {
                    TempData["message"] = Lang.UnexpectedError;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = Lang.ErrorNote;

                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Sends password reset email to user's email
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="usercode">Captcha code (Needed after 3 failed requests)</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPassword(string email, string usercode)
        {

            try
            {
                // Check captcha only after 3 failed requests
                int failCount = Convert.ToInt32(HttpContext.Session.GetInt32("FailCount"));
                if (failCount > 3 && !Captcha.ValidateCaptchaCode("securityCodeResetPass", usercode, HttpContext))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                //Post model to ApiGateway
                var resetJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/ResetPassword", Helpers.Serializers.SerializeJson(new ResetPasswordModel() { email = email }));

                //Parse result
                SimpleResponse resetResponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(resetJson);

                if (resetResponse.Success)
                {
                    failCount++;
                    return base.Json(new SimpleResponse { Success = true, Message = Lang.PasswordResetSuccess });
                }
                else
                {
                    failCount++;
                    if (resetResponse.Message == "Email error")
                    {
                        HttpContext.Session.SetInt32("FailCount", failCount);
                        return base.Json(new SimpleResponse { Success = false, Message = Lang.EmailError });
                    }
                    else
                    {
                        return base.Json(new SimpleResponse { Success = false, Message = Lang.UnexpectedError });
                    }
                }

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return base.Json(new SimpleResponse { Success = false, Message = Lang.UnexpectedError });
            }
        }

        /// <summary>
        /// Opens password reset model from 
        /// </summary>
        /// <param name="str">Encrypted user information in the password reset email</param>
        /// <returns></returns>
        public ActionResult ResetPasswordView(string str)
        {
            try
            {
                //Set password change token into session
                HttpContext.Session.SetString("passwordchangetoken", str);

                //Decrypt information
                string decryptedToken = Helpers.Encryption.DecryptString(str);

                //Check if format is valid
                if (decryptedToken.Split('|').Length > 1)
                {
                    //Check if password renewal expired
                    DateTime emaildate = Convert.ToDateTime(decryptedToken.Split('|')[1]);
                    if (emaildate.AddMinutes(60) < DateTime.Now)
                    {
                        TempData["message"] = Lang.RenewExpired;
                    }
                    else
                    {
                        //Set user's email
                        HttpContext.Session.SetString("passwordchangeemail", decryptedToken.Split('|')[0]);
                        TempData["action"] = "resetpassword";
                    }
                }
                else
                {
                    TempData["message"] = Lang.IncorrectPassReset;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = Lang.ErrorNote;

                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Sets user new password
        /// </summary>
        /// <param name="newpass">New password</param>
        /// <param name="newpassagain">New password confirmation</param>
        /// <param name="usercode">Captcha code (Needed after 3 failed requests)</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPasswordComplete(string newpass, string newpassagain, string usercode)
        {
            try
            {
                // Check captcha only after 3 failed requests
                int failCount = Convert.ToInt32(HttpContext.Session.GetInt32("FailCount"));
                if (failCount > 3 && !Captcha.ValidateCaptchaCode("securityCodeResetPassComplete", usercode, HttpContext))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                //Password match control
                if (newpass != newpassagain)
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.NotCompatiblePass });
                }

                //Password strength control
                if (!Regex.IsMatch(newpass, @"^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])"))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorPasswordMsg });
                }

                //Post model to ApiGateway
                var resetJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/ResetPasswordComplete", Helpers.Serializers.SerializeJson(new ResetCompleteModel() { newPass = newpass, passwordChangeToken = HttpContext.Session.GetString("passwordchangetoken") }));

                //Parse result
                SimpleResponse resetResponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(resetJson);

                if (resetResponse.Success)
                {
                    return base.Json(new SimpleResponse { Success = true, Message = Lang.UpdatePassword });
                }
                else
                {
                    if (resetResponse.Message == "Renew expired")
                    {
                        HttpContext.Session.SetString("passwordchangeemail", "true");

                        return base.Json(new SimpleResponse { Success = false, Message = Lang.RenewExpired });
                    }
                    else
                    {
                        return base.Json(new SimpleResponse { Success = false, Message = Lang.UnexpectedError });
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return base.Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
            }
        }

        /// <summary>
        /// User logout function
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        #endregion

        /// <summary>
        /// Creates captcha image for public forms
        /// This method is called after 3 failed requests
        /// </summary>
        /// <param name="code">Captcha code</param>
        /// <returns>Captcha image</returns>
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage(string code)
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString(code, result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

    }
}
