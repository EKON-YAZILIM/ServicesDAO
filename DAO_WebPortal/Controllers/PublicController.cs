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

namespace DAO_WebPortal.Controllers
{
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
            ViewBag.HeaderSubTitle = "We designed a brand-new cool design and lots of features, the latest version of the template supports advanced block base scenarios, and more.";


            return View();
        }

        [Route("Contact")]
        public IActionResult Contact()
        {
            ViewBag.HeaderTitle = "Contact";
            ViewBag.HeaderSubTitle = "We designed a brand-new cool design and lots of features, the latest version of the template supports advanced block base scenarios, and more.";

            return View();
        }

        [Route("RFP")]
        public IActionResult RFP()
        {
            ViewBag.HeaderTitle = "Request for Proposal (RFP)";
            ViewBag.HeaderSubTitle = "We designed a brand-new cool design and lots of features, the latest version of the template supports advanced block base scenarios, and more.";


            return View();
        }

        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }

        #endregion

        #region Login & Register

        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserID") != null && HttpContext.Session.GetInt32("UserID") > 0)
            {
                return RedirectToAction("Dashboard", "Main");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login(string email, string password, string usercode)
        {
            try
            {
                int failCount = Convert.ToInt32(HttpContext.Session.GetInt32("FailCount"));

                if (failCount > 4 && !Utility.Captcha.ValidateCaptchaCode("securityCode1", usercode, HttpContext))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                LoginResponse loginModel = new LoginResponse();
                string ip = Methods.GetClientIpAddress(HttpContext);
                string port = Methods.GetClientPort(HttpContext);
                LoginModel LoginModelPost = new LoginModel() { email = email, pass = password, ip = ip, port = port, application = Helpers.Constants.Enums.AppNames.DAO_WebPortal };


                var loginJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/Login", Helpers.Serializers.SerializeJson(LoginModelPost));
                loginModel = Helpers.Serializers.DeserializeJson<LoginResponse>(loginJson);
       
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

                    return Json(new AjaxResponse { Success = true, Message = Lang.SuccessLogin });
                }
                else
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.ErrorUsernamePassword });
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return Json(new AjaxResponse { Success = false, Message = Lang.ErrorNote });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Register(string email, string username, string namesurname, string gsm, string password, string repass, string usercode)
        {
            int failCount = Convert.ToInt32(HttpContext.Session.GetInt32("FailCount"));

            try
            {
                //CAPTCHA CONTROL
                if (failCount > 4 && !Captcha.ValidateCaptchaCode("securityCode2", usercode, HttpContext))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                //PASSWORD MATCH CONTROL
                if (password != repass)
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.NotCompatiblePass });
                }

                //PASSWORD STRENGTH CONTROL
                if (!Regex.IsMatch(password, @"^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])"))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.ErrorPasswordMsg });
                }

                string ip = Methods.GetClientIpAddress(HttpContext);
                string port = Methods.GetClientPort(HttpContext);

                //REGISTER
                var registerJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/Register", Helpers.Serializers.SerializeJson(new RegisterModel() { email = email, username = username, namesurname = namesurname, password = password, ip = ip, port = port }));
                AjaxResponse registerResponse = Helpers.Serializers.DeserializeJson<AjaxResponse>(registerJson);

                if (registerResponse.Success == false)
                {
                    failCount++;
                    if (registerResponse.Message == "Username already exists.")
                    {
                        return Json(new AjaxResponse { Success = false, Message = Lang.ErrorUserMsg });
                    }
                    else if (registerResponse.Message == "Email already exists.")
                    {
                        return Json(new AjaxResponse { Success = false, Message = Lang.ErrorMailMsg });
                    }
                    else if (registerResponse.Message == "User post error")
                    {
                        return Json(new AjaxResponse { Success = false, Message = Lang.UnexpectedError });
                    }
                    else
                    {
                        return Json(new AjaxResponse { Success = false, Message = Lang.ErrorNote });
                    }
                }
                else
                {
                    return Json(new AjaxResponse { Success = true, Message = Lang.RegisterEmailSent });
                }
            }
            catch (Exception ex)
            {
                failCount++;
                HttpContext.Session.SetInt32("FailCount", failCount);
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return Json(new AjaxResponse { Success = false, Message = Lang.ErrorNote });
            }


        }

        public ActionResult RegisterCompleteView(string str)
        {
            try
            {
                HttpContext.Session.SetString("passwordchangetoken", str);

                var completeJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/RegisterComplete?registerCode=" + str);
                AjaxResponse completeResponse = Helpers.Serializers.DeserializeJson<AjaxResponse>(completeJson);

                if (completeResponse.Success)
                {
                    TempData["message"] = Lang.Successful;
                    TempData["message2"] = Lang.RegisterComplete;
                }
                else
                {
                    TempData["message"] = Lang.Error;
                    TempData["message2"] = Lang.UnexpectedError;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = Lang.Error;
                TempData["message2"] = Lang.ErrorNote;

                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPassword(string email, string usercode)
        {
            int failCount = Convert.ToInt32(HttpContext.Session.GetInt32("FailCount"));

            try
            {
                if (failCount > 4 && !Captcha.ValidateCaptchaCode("securityCode3", usercode, HttpContext))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                var resetJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/ResetPassword", Helpers.Serializers.SerializeJson(new ResetPasswordModel() { email = email }));
                AjaxResponse resetResponse = Helpers.Serializers.DeserializeJson<AjaxResponse>(resetJson);

                if (resetResponse.Success)
                {
                    failCount++;
                    return Json(new AjaxResponse { Success = true, Message = Lang.PasswordResetSuccess });
                }
                else
                {
                    failCount++;
                    if (resetResponse.Message == "Email error")
                    {
                        HttpContext.Session.SetInt32("FailCount", failCount);
                        return Json(new AjaxResponse { Success = false, Message = Lang.EmailError });
                    }
                    else
                    {
                        return Json(new AjaxResponse { Success = false, Message = Lang.UnexpectedError });
                    }
                }

            }
            catch (Exception ex)
            {
                failCount++;
                HttpContext.Session.SetInt32("FailCount", failCount);
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return Json(new AjaxResponse { Success = false, Message = Lang.UnexpectedError });
            }
        }

        public ActionResult ResetPasswordView(string str)
        {
            try
            {
                HttpContext.Session.SetString("passwordchangetoken", str);

                string email = Helpers.Encryption.DecryptString(str);
                if (email.Split('|').Length > 1)
                {
                    DateTime emaildate = Convert.ToDateTime(email.Split('|')[1]);
                    if (emaildate.AddMinutes(15) < DateTime.Now)
                    {
                        TempData["message"] = Lang.Error;
                        TempData["message2"] = Lang.RenewExpired;
                    }
                    else
                    {
                        HttpContext.Session.SetString("passwordchangeemail", email.Split('|')[0]);
                        TempData["action"] = "resetpassword";
                    }
                }
                else
                {
                    TempData["message"] = Lang.Error;
                    TempData["message2"] = Lang.IncorrectPassReset;
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = Lang.Error;
                TempData["message2"] = Lang.ErrorNote;

                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetComplete(string newpass, string newpassagain, string usercode)
        {
            int failCount = Convert.ToInt32(HttpContext.Session.GetInt32("FailCount"));

            try
            {
                //CAPTCHA CONTROL
                if (failCount > 4 && !Captcha.ValidateCaptchaCode("securityCode4", usercode, HttpContext))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.WrongErrorCodeEntered });
                }

                //PASSWORD STRENGTH CONTROL
                if (!Regex.IsMatch(newpass, @"^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])"))
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.ErrorPasswordMsg });
                }

                //PASSWORD MATCH CONTROL
                if (newpass != newpassagain)
                {
                    failCount++;
                    HttpContext.Session.SetInt32("FailCount", failCount);
                    return Json(new AjaxResponse { Success = false, Message = Lang.NotCompatiblePass });

                }

                var resetJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/PublicActions/Identity/ResetPasswordComplete", Helpers.Serializers.SerializeJson(new ResetCompleteModel() { newPass = newpass, passwordChangeToken = HttpContext.Session.GetString("passwordchangetoken") }));
                AjaxResponse resetResponse = Helpers.Serializers.DeserializeJson<AjaxResponse>(resetJson);

                if (resetResponse.Success)
                {
                    return Json(new AjaxResponse { Success = true, Message = Lang.UpdatePassword });
                }
                else
                {
                    if (resetResponse.Message == "Renew expired")
                    {
                        HttpContext.Session.SetString("passwordchangeemail", "true");

                        return Json(new AjaxResponse { Success = false, Message = Lang.RenewExpired });
                    }
                    else
                    {
                        return Json(new AjaxResponse { Success = false, Message = Lang.UnexpectedError });
                    }
                }
            }
            catch (Exception ex)
            {
                failCount++;
                HttpContext.Session.SetInt32("FailCount", failCount);
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return Json(new AjaxResponse { Success = false, Message = Lang.ErrorNote });
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        #endregion

      
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
