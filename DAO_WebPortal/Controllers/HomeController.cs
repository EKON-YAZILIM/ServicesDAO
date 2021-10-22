using DAO_WebPortal.Models;
using DAO_WebPortal.Providers;
using Helpers.Models.WebsiteViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_WebPortal.Controllers
{
    [AuthorizeUser]
    public class HomeController : Controller
    {
        #region Views
        
        [Route("Home")]
        public IActionResult Index()
        {
            var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoard?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
            var loginModel = Helpers.Serializers.DeserializeJson<GetDashBoardViewModel>(url);

            return View(loginModel);
        }

        [Route("My-Jobs")]
        public IActionResult My_Jobs()
        {
            return View();
        }

        [Route("All-Jobs")]
        public IActionResult All_Jobs()
        {
            return View();
        }

        [Route("Auctions")]
        public IActionResult Auctions()
        {
            return View();
        }

        [Route("Votes")]
        public IActionResult Votes()
        {
            return View();
        }

        [Route("Reputation-History")]
        public IActionResult Reputation_History()
        {
            return View();
        }

        [Route("Payments-History")]
        public IActionResult Payments_History()
        {
            return View();
        }

        [Route("User-Profile")]
        public IActionResult User_Profile()
        {
            return View();
        }

        [Route("Contact-Help")]
        public IActionResult Contact_Help()
        {
            return View();
        }

        [Route("Job-Detail/{Job}")]
        public IActionResult Job_Detail(int Job)
        {
            return View();
        }

        [Route("Vote-Detail/{VoteID}")]
         public IActionResult Vote_Detail(int VoteID)
        {
            return View();
        }
        #endregion

        #region UserSerttings
        [HttpGet]
        public JsonResult SetCookie(string src)
        {
            CookieOptions cookies = new CookieOptions();
            cookies.Expires = DateTime.Now.AddDays(90);
            Response.Cookies.Append("theme", src, cookies);
            return Json("");
        }
        #endregion
    }
}
